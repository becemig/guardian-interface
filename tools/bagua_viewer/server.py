"""
BaguaViewer server.py -- FastAPI WebSocket backend
Streams JSON physics frames to Three.js frontend.
Run: uvicorn server:app --host 0.0.0.0 --port 8765
"""

import sys, json, asyncio, numpy as np
from fastapi import FastAPI, WebSocket, WebSocketDisconnect
from fastapi.staticfiles import StaticFiles
from fastapi.responses import FileResponse

sys.path.insert(0, "/home/becemig/GodotProjects/guardian-interface/tools")
from bagua_physics.curvature_field import compute_curvature_stream
from bagua_physics.icr_solver import compute_icr_stream
from bagua_physics.fascial_activation import compute_fascial_stream
from bagua_physics.five_element_map import compute_five_element_stream
from bagua_physics.ba_gang_diagnosis import compute_ba_gang_stream
from bagua_physics.mechanotransduction import compute_mechanotransduction_stream
from bagua_physics.neuro_fascial import compute_neuro_fascial_stream
from bagua_physics.channel_nerve_map import compute_channel_stream, CHANNEL_DEFS

app = FastAPI()
app.mount("/static", StaticFiles(directory="static"), name="static")


CHAIN_JOINTS_IDX = [5,6,7,8,9,10,11,12,13,14,15,16]

# Anatomical rest offsets for 12 chain joints (x, y, z) in metres
# Order: R_ankle, R_knee, R_hip, L_hip, L_knee, L_ankle,
#        sacrum, L1, T1, C7, R_shoulder, L_shoulder
REST_OFFSETS = [
    ( 0.12, 0.08, 0.0),  # 0 R_ankle
    ( 0.12, 0.48, 0.0),  # 1 R_knee
    ( 0.10, 0.92, 0.0),  # 2 R_hip
    (-0.10, 0.92, 0.0),  # 3 L_hip
    (-0.12, 0.48, 0.0),  # 4 L_knee
    (-0.12, 0.08, 0.0),  # 5 L_ankle
    ( 0.00, 1.00, 0.0),  # 6 sacrum
    ( 0.00, 1.28, 0.0),  # 7 L1
    ( 0.00, 1.62, 0.0),  # 8 T1
    ( 0.00, 1.82, 0.0),  # 9 C7
    ( 0.22, 1.58, 0.0),  # 10 R_shoulder
    (-0.22, 1.58, 0.0),  # 11 L_shoulder
]

def _synthetic_positions(T=240, J=20):
    """
    Anatomically proportioned circle-walk skeleton.
    Body orbits at radius 1.8m. Limbs maintain correct relative positions.
    CHAIN_JOINTS order: R_ankle(5), R_knee(6), R_hip(7), R_shldr(8),
      R_elbow(9), R_wrist(10), L_wrist(11), L_elbow(12), L_shldr(13),
      L_hip(14), L_knee(15), L_ankle(16)
    Mapped to REST_OFFSETS indices 0-11.
    """
    t = np.linspace(0, 4*np.pi, T)
    pos = np.zeros((T, J, 3))
    # Pelvis orbits circle at 1.8m radius, height 1.0m
    px = 1.8 * np.cos(t)
    pz = 1.8 * np.sin(t)
    py = np.full(T, 1.0)
    # Forward direction = tangent of circle = (-sin, 0, cos)
    fwd_x = -np.sin(t)
    fwd_z =  np.cos(t)
    # Right direction = perpendicular = (cos, 0, sin)
    rt_x = np.cos(t)
    rt_z = np.sin(t)
    # Gait: alternating leg lift, 2 full steps per revolution
    step = np.sin(2 * t)          # gait cycle
    ankle_lift = np.clip(step, 0, 1) * 0.18   # R ankle lifts
    ankle_lift_L = np.clip(-step, 0, 1) * 0.18 # L ankle lifts
    # Arm swing opposite to legs
    arm_swing = np.sin(2 * t) * 0.12
    # --- Build each joint in world space ---
    # REST anatomy (right side positive x, up positive y):
    # Pelvis center = origin reference
    hip_w  = 0.11   # half hip width
    shldr_w = 0.19  # half shoulder width
    # R hip
    rh_x = px + rt_x * hip_w
    rh_z = pz + rt_z * hip_w
    rh_y = py
    # L hip
    lh_x = px - rt_x * hip_w
    lh_z = pz - rt_z * hip_w
    lh_y = py
    # R knee (0.42m below hip, slight forward bend)
    rk_x = rh_x + fwd_x * 0.04 * np.abs(step)
    rk_z = rh_z + fwd_z * 0.04 * np.abs(step)
    rk_y = rh_y - 0.42
    # L knee
    lk_x = lh_x - fwd_x * 0.04 * np.abs(step)
    lk_z = lh_z - fwd_z * 0.04 * np.abs(step)
    lk_y = lh_y - 0.42
    # R ankle
    ra_x = rk_x - fwd_x * 0.03
    ra_z = rk_z - fwd_z * 0.03
    ra_y = rk_y - 0.40 + ankle_lift
    # L ankle
    la_x = lk_x + fwd_x * 0.03
    la_z = lk_z + fwd_z * 0.03
    la_y = lk_y - 0.40 + ankle_lift_L
    # Sacrum (midpoint of hips + 0.02 up)
    sc_x = px
    sc_z = pz
    sc_y = py + 0.02
    # T1 (0.58m above sacrum)
    t1_x = px
    t1_z = pz
    t1_y = py + 0.58
    # C7 (0.22m above T1)
    c7_x = px
    c7_z = pz
    c7_y = t1_y + 0.22
    # R shoulder
    rs_x = px + rt_x * shldr_w
    rs_z = pz + rt_z * shldr_w
    rs_y = t1_y + 0.04
    # L shoulder
    ls_x = px - rt_x * shldr_w
    ls_z = pz - rt_z * shldr_w
    ls_y = t1_y + 0.04
    # R elbow (0.28m below shoulder, arm swings forward/back)
    re_x = rs_x + fwd_x * arm_swing
    re_z = rs_z + fwd_z * arm_swing
    re_y = rs_y - 0.28
    # L elbow (opposite swing)
    le_x = ls_x - fwd_x * arm_swing
    le_z = ls_z - fwd_z * arm_swing
    le_y = ls_y - 0.28
    # R wrist
    rw_x = re_x + fwd_x * arm_swing * 0.5
    rw_z = re_z + fwd_z * arm_swing * 0.5
    rw_y = re_y - 0.24
    # L wrist
    lw_x = le_x - fwd_x * arm_swing * 0.5
    lw_z = le_z - fwd_z * arm_swing * 0.5
    lw_y = le_y - 0.24
    # Map to CHAIN_JOINTS_IDX = [5,6,7,8,9,10,11,12,13,14,15,16]
    # idx 5=R_ankle, 6=R_knee, 7=R_hip, 8=R_shldr, 9=R_elbow, 10=R_wrist
    # idx 11=L_wrist, 12=L_elbow, 13=L_shldr, 14=L_hip, 15=L_knee, 16=L_ankle
    coords = [
        (ra_x, ra_y, ra_z),   # 5  R ankle
        (rk_x, rk_y, rk_z),   # 6  R knee
        (rh_x, rh_y, rh_z),   # 7  R hip
        (rs_x, rs_y, rs_z),   # 8  R shoulder
        (re_x, re_y, re_z),   # 9  R elbow
        (rw_x, rw_y, rw_z),   # 10 R wrist
        (lw_x, lw_y, lw_z),   # 11 L wrist
        (le_x, le_y, le_z),   # 12 L elbow
        (ls_x, ls_y, ls_z),   # 13 L shoulder
        (lh_x, lh_y, lh_z),   # 14 L hip
        (lk_x, lk_y, lk_z),   # 15 L knee
        (la_x, la_y, la_z),   # 16 L ankle
    ]
    for k, ji in enumerate(CHAIN_JOINTS_IDX):
        pos[:, ji, 0] = coords[k][0]
        pos[:, ji, 1] = coords[k][1]
        pos[:, ji, 2] = coords[k][2]
    return pos


CHAIN_JOINTS = [5,6,7,8,9,10,11,12,13,14,15,16]

def _heat(v):
    if v < 0.5:
        t = v * 2
        return [round(0.125+t*0.725,3), round(0.502+t*0.048,3), round(0.549-t*0.439,3)]
    else:
        t = (v-0.5)*2
        return [round(0.85+t*(-0.219),3), round(0.55+t*(-0.342),3), round(0.11+t*0.157,3)]

def _icr_vecs(joint_pos, name, icr_by_name):
    """
    Return centripetal and centrifugal unit vectors + lambda for a joint.
    centripetal: joint -> ICR (inward)
    centrifugal: ICR -> joint (outward)
    """
    null = {"valid": False, "cp": [0,0,0], "cf": [0,0,0], "lam": 0.0, "mag": 0.0}
    if name is None:
        return null
    jicr = icr_by_name.get(name)
    if jicr is None:
        return null
    jp = np.array(joint_pos)
    ip = jicr.icr_pos
    diff = ip - jp
    dist = float(np.linalg.norm(diff))
    if dist < 1e-6:
        return null
    cp = diff / dist   # centripetal unit vector (toward ICR)
    cf = -cp           # centrifugal unit vector (away from ICR)
    lam = float(np.clip(jicr.lambda_val / 5.0, 0.0, 1.0))  # normalized
    return {
        "valid": True,
        "cp": [round(float(v),4) for v in cp],
        "cf": [round(float(v),4) for v in cf],
        "lam": round(lam, 4),
        "mag": round(float(np.clip(dist/3.0, 0.0, 1.0)), 4),
    }


CHAIN_NAMES = [
    None,            # right_ankle -- no ICR pair
    None,            # right_knee
    None,            # right_hip
    "shoulder_girdle",  # right_shoulder
    "right_elbow",
    "right_wrist",
    "left_wrist",
    "left_elbow",
    "shoulder_girdle",  # left_shoulder
    None,            # left_hip
    None,            # left_knee
    None,            # left_ankle
]


def _at_color(v):
    return [0.12 + v*0.73, 0.08 + v*0.47, 0.01 + v*0.09]

def _jj_color(v):
    return [0.02 + v*0.28, 0.18 + v*0.47, 0.22 + v*0.53]

def _build_frames():
    pos = _synthetic_positions()
    frames = compute_curvature_stream(pos)
    icr_frames = compute_icr_stream(pos)
    T_len = pos.shape[0]
    f_long_proxy = np.zeros((T_len, 16))
    for fi, cf in enumerate(frames):
        for k, ji in enumerate(CHAIN_JOINTS_IDX):
            if ji < 16:
                f_long_proxy[fi, ji] = float(cf.attunement[k])
    fasc_frames = compute_fascial_stream(f_long_proxy)
    fe_frames = compute_five_element_stream(pos)
    # Extract per-frame arrays for ba_gang
    bg_attune  = [float(cf.global_attunement) for cf in frames]
    bg_lambda  = [float(icr_frames[i].mean_lambda) for i in range(len(frames))]
    bg_dom_el  = [fe_frames[i].dominant_element for i in range(len(frames))]
    bg_sbl     = [float(fasc_frames[i].anatomy_trains.get("SBL",0)) for i in range(len(frames))]
    bg_bljj    = [float(fasc_frames[i].jing_jin.get("BL_JJ",0)) for i in range(len(frames))]
    bg_stab    = [float(icr_frames[i].stability_index) for i in range(len(frames))]
    bg_sheng   = [float(fe_frames[i].sheng_flow_score) for i in range(len(frames))]
    bg_vol     = [float(np.sum(np.abs(cf.kappa))) for cf in frames]
    mech_frames = compute_mechanotransduction_stream(frames, icr_frames)
    neuro_frames = compute_neuro_fascial_stream(mech_frames, frames)
    channel_frames = compute_channel_stream(frames, neuro_frames)
    ba_frames  = compute_ba_gang_stream(
        bg_attune, bg_lambda, bg_dom_el,
        bg_sbl, bg_bljj, bg_stab,
        bg_sheng, bg_vol)
    all_A = np.array([cf.attunement for cf in frames])
    A_max = float(np.percentile(all_A, 98)) or 1.0
    Ag_vals = np.array([cf.global_attunement for cf in frames])
    Ag_max = float(np.percentile(Ag_vals, 98)) or 1.0
    out = []
    for i, cf in enumerate(frames):
        joints = []
        icf = icr_frames[i]
        # Layer 4: velocity and acceleration per joint
        vel_list = []
        acc_list = []
        for k, ji in enumerate(CHAIN_JOINTS_IDX):
            if i == 0:
                v = pos[1,ji] - pos[0,ji]
                a = np.zeros(3)
            elif i == len(frames)-1:
                v = pos[-1,ji] - pos[-2,ji]
                a = np.zeros(3)
            else:
                v = (pos[i+1,ji] - pos[i-1,ji]) * 0.5
                a = pos[i+1,ji] - 2*pos[i,ji] + pos[i-1,ji]
            vel_list.append([round(float(x),4) for x in v])
            acc_list.append([round(float(x),4) for x in a])
        ff = fasc_frames[i]
        fe = fe_frames[i]
        bg = ba_frames[i]
        mf = mech_frames[i]
        nf = neuro_frames[i]
        chf = channel_frames[i]
        icr_by_name = {jicr.name: jicr for jicr in icf.joints if jicr.icr_valid}
        for k, j in enumerate(CHAIN_JOINTS_IDX):
            A_norm = float(np.clip(cf.attunement[k] / A_max, 0.0, 1.0))
            joints.append({
                "x": round(float(pos[i, j, 0]), 4),
                "y": round(float(pos[i, j, 1]), 4),
                "z": round(float(pos[i, j, 2]), 4),
                "kappa": round(float(cf.kappa[k]), 4),
                "A": round(A_norm, 4),
                "rgb": _heat(A_norm),
                "icr": _icr_vecs(pos[i, j], CHAIN_NAMES[k], icr_by_name),
            })
        out.append({
            "frame": i,
            "global_A": round(float(cf.global_attunement / Ag_max), 4),
            "joints": joints,
            # Layer 5: ground reaction forces at feet (indices 0=R_ankle, 11=L_ankle)
            "grf": {
                "R": [round(float(x),4) for x in (pos[i,CHAIN_JOINTS_IDX[0]+1] - pos[i,CHAIN_JOINTS_IDX[0]])]
                     if i < len(frames)-1 else [0.0,0.0,0.0],
                "L": [round(float(x),4) for x in (pos[i,CHAIN_JOINTS_IDX[11]+1] - pos[i,CHAIN_JOINTS_IDX[11]])]
                     if i < len(frames)-1 else [0.0,0.0,0.0],
                "mag": round(float(frames[i].global_attunement), 4),
            },
            "vel": vel_list,
            "acc": acc_list,
            "wave": round(float(frames[i].global_attunement), 4),
            "channel": {
                "activation": {k: round(v,4) for k,v in chf.activation.items()},
                "element_load": {k: round(v,4) for k,v in chf.element_load.items()},
                "asymmetry": {k: round(v,4) for k,v in chf.asymmetry.items()},
                "dominant_channel": chf.dominant_channel,
                "dominant_element": chf.dominant_element,
                "yin_total": round(chf.yin_total, 4),
                "yang_total": round(chf.yang_total, 4),
            },
            "neuro": {
                "ruffini":  [round(r,4) for r in nf.ruffini],
                "pacini":   [round(p,4) for p in nf.pacini],
                "golgi":    nf.golgi,
                "spindle":  [round(s,4) for s in nf.spindle],
                "pulse":    nf.afferent_pulse,
                "prop":     round(nf.propriocept_field, 4),
                "autonomic": round(nf.autonomic_tone, 4),
                "receptor": nf.dominant_receptor,
            },
            "mech": {
                "stress": [round(s,4) for s in mf.stress],
                "piezo":  [round(p,4) for p in mf.piezo],
                "integrin": mf.integrin_active,
                "yap_taz":  mf.yap_taz_active,
                "remodel":  mf.remodel_zone,
                "mech_index": mf.mech_index,
                "dominant_zone": mf.dominant_zone,
            },
            "ba_gang": {
                "yin": bg.yin, "yang": bg.yang,
                "interior": bg.interior, "exterior": bg.exterior,
                "cold": bg.cold, "hot": bg.hot,
                "deficient": bg.deficient, "excess": bg.excess,
                "pattern": bg.pattern, "confidence": bg.confidence,
            },
            "five_element": {
                "scores": {k: round(float(v),4) for k,v in fe.element_scores.items()},
                "dominant": fe.dominant_element,
                "resonant_herbs": fe.resonant_herbs,
            },
            "fascial": {
                "at": {k: {"act": round(v,4), "col": [round(c,3) for c in _at_color(v)]} for k,v in ff.anatomy_trains.items()},
                "jj": {k: {"act": round(v,4), "col": [round(c,3) for c in _jj_color(v)]} for k,v in ff.jing_jin.items()},
                "yjj_stage": ff.yjj_current_stage,
            },
        })
    return out


# Pre-compute once at startup
FRAMES = []

@app.on_event("startup")
async def startup():
    global FRAMES
    FRAMES = _build_frames()
    print(f"BaguaViewer ready -- {len(FRAMES)} frames loaded")


@app.get("/")
async def root():
    return FileResponse("static/index.html")



@app.get("/frame/{idx}")
async def get_frame(idx: int):
    if not FRAMES:
        return {"error": "frames not ready"}
    idx = idx % len(FRAMES)
    return FRAMES[idx]

@app.get("/frames/count")
async def get_frame_count():
    return {"count": len(FRAMES)}

@app.get("/channel_defs")
async def get_channel_defs():
    from bagua_physics.channel_nerve_map import CHANNEL_DEFS
    return CHANNEL_DEFS

@app.websocket("/ws")
async def websocket_stream(ws: WebSocket):
    await ws.accept()
    idx = 0
    playing = True
    try:
        await ws.send_json({"type":"init","total":len(FRAMES)})
        async def pusher():
            nonlocal idx, playing
            while True:
                if playing:
                    frame = dict(FRAMES[idx])
                    frame["type"] = "frame"
                    await ws.send_json(frame)
                    idx = (idx + 1) % len(FRAMES)
                await asyncio.sleep(1/15)
        task = asyncio.create_task(pusher())
        try:
            while True:
                msg = await ws.receive_json()
                cmd = msg.get("cmd","")
                if cmd == "seek":
                    idx = int(msg.get("frame",0)) % len(FRAMES)
                    playing = False
                elif cmd == "play":
                    playing = True
                elif cmd == "pause":
                    playing = False
                elif cmd == "next":
                    idx = (idx + 1) % len(FRAMES)
        finally:
            task.cancel()
    except WebSocketDisconnect:
        pass
    except Exception as e:
        print("WS error:", e)
