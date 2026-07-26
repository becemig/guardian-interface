"use strict";

// -----------------------------------------------------------
// Scene setup
// -----------------------------------------------------------
const canvas   = document.getElementById("c");
const renderer = new THREE.WebGLRenderer({ canvas, antialias: true });
renderer.setPixelRatio(window.devicePixelRatio);
renderer.setSize(window.innerWidth, window.innerHeight);
renderer.toneMapping = THREE.ACESFilmicToneMapping;
renderer.toneMappingExposure = 1.1;

const scene  = new THREE.Scene();
scene.background = new THREE.Color(0x0a0a0e);
scene.fog = new THREE.Fog(0x0a0a0e, 8, 25);

const camera = new THREE.PerspectiveCamera(55, window.innerWidth/window.innerHeight, 0.01, 50);
camera.position.set(6, 2.0, 6);
camera.lookAt(0, 0.9, 0);

// Ambient + rim light
scene.add(new THREE.AmbientLight(0x1a1a2e, 2.0));
const rimL = new THREE.DirectionalLight(0x4F98A3, 1.4);
rimL.position.set(-4, 6, -3); scene.add(rimL);

// Grid floor
const grid = new THREE.GridHelper(12, 24, 0x222230, 0x18181e);
scene.add(grid);

// Simple orbit-like mouse drag
let isDragging=false, prevX=0, prevY=0;
let theta=Math.PI/4, phi=Math.PI/8, camR=5.5;
function updateCamera(){
  camera.position.set(
    camR*Math.sin(theta)*Math.cos(phi),
    camR*Math.sin(phi)+0.9,
    camR*Math.cos(theta)*Math.cos(phi)
  );
  camera.lookAt(0,0.9,0);
}
canvas.addEventListener("mousedown", e=>{isDragging=true; prevX=e.clientX; prevY=e.clientY;});
canvas.addEventListener("mouseup",   ()=>isDragging=false);
canvas.addEventListener("mousemove", e=>{
  if(!isDragging)return;
  theta -= (e.clientX-prevX)*0.008;
  phi    = Math.max(-0.2, Math.min(1.2, phi+(e.clientY-prevY)*0.006));
  prevX=e.clientX; prevY=e.clientY;
  updateCamera();
});
canvas.addEventListener("wheel", e=>{
  camR = Math.max(2, Math.min(18, camR+e.deltaY*0.01));
  updateCamera();
}, {passive:true});
window.addEventListener("resize", ()=>{
  camera.aspect = window.innerWidth/window.innerHeight;
  camera.updateProjectionMatrix();
  renderer.setSize(window.innerWidth, window.innerHeight);
});


// -----------------------------------------------------------
// Skeleton geometry -- joints + bones
// -----------------------------------------------------------
const JOINT_COUNT = 12;
const BONES = [
  [0,1],[1,2],         // R ankle-knee-hip
  [11,10],[10,9],      // L ankle-knee-hip
  [2,9],               // R hip -- L hip
  [3,8],               // R shoulder -- L shoulder
  [2,3],[9,8],         // R hip -- R shoulder, L hip -- L shoulder
  [3,4],[4,5],         // R shoulder-elbow-wrist
  [8,7],[7,6],         // L shoulder-elbow-wrist
];

// Joint spheres
const jointMat = new THREE.MeshStandardMaterial({vertexColors:true, roughness:0.35, metalness:0.4});
const jointGeo = new THREE.SphereGeometry(0.045, 10, 10);
const joints = [];
for(let j=0; j<JOINT_COUNT; j++){
  const m = new THREE.MeshStandardMaterial({color:0x20808D, roughness:0.35, metalness:0.4});
  const mesh = new THREE.Mesh(jointGeo, m);
  scene.add(mesh);
  joints.push(mesh);
}

// Bone lines using BufferGeometry
const boneLines = [];
BONES.forEach(([a,b])=>{
  const geo = new THREE.BufferGeometry();
  const pos = new Float32Array(6);
  geo.setAttribute("position", new THREE.BufferAttribute(pos,3));
  const col = new Float32Array(6);
  geo.setAttribute("color", new THREE.BufferAttribute(col,3));
  const mat = new THREE.LineBasicMaterial({vertexColors:true, linewidth:1.5});
  const line = new THREE.Line(geo, mat);
  scene.add(line);
  boneLines.push({line, geo, a, b});
});

// Glow halo per joint (PointLight-lite using sprite)
const glows = [];
const glowTex = (()=>{
  const sz=64, cv=document.createElement("canvas");
  cv.width=cv.height=sz;
  const ctx=cv.getContext("2d");
  const g=ctx.createRadialGradient(32,32,0,32,32,32);
  g.addColorStop(0,"rgba(255,255,255,0.9)");
  g.addColorStop(1,"rgba(255,255,255,0)");
  ctx.fillStyle=g; ctx.fillRect(0,0,sz,sz);
  return new THREE.CanvasTexture(cv);
})();
for(let j=0; j<JOINT_COUNT; j++){
  const mat = new THREE.SpriteMaterial({map:glowTex, color:0x20808D,
    transparent:true, opacity:0.22, blending:THREE.AdditiveBlending, depthWrite:false});
  const sp = new THREE.Sprite(mat);
  sp.scale.set(0.28,0.28,1);
  scene.add(sp);
  glows.push(sp);
}




// -----------------------------------------------------------
// Layer 2 -- ICR force vectors (ArrowHelper per joint x2)
// -----------------------------------------------------------
const CP_COLOR = 0x4F98A3;   // centripetal -- teal -- inward
const CF_COLOR = 0xDA7101;   // centrifugal -- amber -- outward
const arrowsCP = [];
const arrowsCF = [];
const ARROW_SCALE = 1.1;
for(let j=0; j<JOINT_COUNT; j++){
  const origin = new THREE.Vector3();
  const dir    = new THREE.Vector3(0,1,0);
  const acp = new THREE.ArrowHelper(dir, origin, ARROW_SCALE, CP_COLOR, 0.12, 0.07);
  const acf = new THREE.ArrowHelper(dir, origin, ARROW_SCALE, CF_COLOR, 0.12, 0.07);
  acp.visible = false;
  acf.visible = false;
  scene.add(acp);
  scene.add(acf);
  arrowsCP.push(acp);
  arrowsCF.push(acf);
}
let showLayer2 = true;

// Layer 4 -- Limb physics vectors
let showLayer4 = true;
const VEL_COLOR = 0xFFFFFF;
const ACC_COLOR = 0xD163A7;
const velArrows = [];
const accArrows = [];
for(let k = 0; k < 12; k++){
  const va = new THREE.ArrowHelper(
    new THREE.Vector3(0,1,0), new THREE.Vector3(0,0,0),
    0.001, VEL_COLOR, 0.10, 0.06);
  va.visible = false;
  scene.add(va);
  velArrows.push(va);
  const aa = new THREE.ArrowHelper(
    new THREE.Vector3(0,1,0), new THREE.Vector3(0,0,0),
    0.001, ACC_COLOR, 0.10, 0.06);
  aa.visible = false;
  scene.add(aa);
  accArrows.push(aa);
}
// Wave: tube along chain that pulses
const wavePts = Array.from({length:12}, ()=>new THREE.Vector3());
const waveCurve = new THREE.CatmullRomCurve3(wavePts);
const waveGeo = new THREE.TubeGeometry(waveCurve, 30, 0.012, 5, false);
const waveMat = new THREE.MeshBasicMaterial({
  color: 0x4F98A3, transparent: true, opacity: 0.0,
  blending: THREE.AdditiveBlending, depthWrite: false
});
const waveMesh = new THREE.Mesh(waveGeo, waveMat);
scene.add(waveMesh);

// Layer 5 -- Ground vector field + environmental sphere
let showLayer5 = true;
const GRF_COLOR = 0x6DAA45;
// Ground reaction arrows at L and R ankle
const grfR = new THREE.ArrowHelper(new THREE.Vector3(0,1,0), new THREE.Vector3(0,0,0), 0.001, GRF_COLOR, 0.12, 0.07);
const grfL = new THREE.ArrowHelper(new THREE.Vector3(0,1,0), new THREE.Vector3(0,0,0), 0.001, GRF_COLOR, 0.12, 0.07);
grfR.visible = false; grfL.visible = false;
scene.add(grfR); scene.add(grfL);
// Gravity reference
const gravArrow = new THREE.ArrowHelper(new THREE.Vector3(0,-1,0), new THREE.Vector3(0,1.5,0), 0.6, 0xBAB9B4, 0.12, 0.07);
scene.add(gravArrow);
// Environmental sphere
const envSphGeo = new THREE.SphereGeometry(2.2, 24, 16);
const envSphMat = new THREE.MeshBasicMaterial({
  color: 0x4F98A3, wireframe: true,
  transparent: true, opacity: 0.04,
  blending: THREE.AdditiveBlending, depthWrite: false
});
const envSphere = new THREE.Mesh(envSphGeo, envSphMat);
scene.add(envSphere);

// Layer 6 -- Five element orbital rings
let showLayer6 = true;
const FE_ELEMENTS = ["Wood","Fire","Earth","Metal","Water"];
const FE_COLORS = [0x6DAA45, 0xA13544, 0xE8AF34, 0xCDCCCA, 0x006494];
const FE_TILT = [0.3, 0.8, 0.0, 0.5, 1.1];
const FE_SPEED = [0.008, 0.013, 0.005, 0.010, 0.007];
const feRings = [];
for(let i=0; i<5; i++){
  const geo = new THREE.TorusGeometry(1.6 + i*0.12, 0.008, 6, 64);
  const mat = new THREE.MeshBasicMaterial({
    color: FE_COLORS[i], transparent: true, opacity: 0.0,
    blending: THREE.AdditiveBlending, depthWrite: false
  });
  const ring = new THREE.Mesh(geo, mat);
  ring.rotation.x = FE_TILT[i];
  ring.rotation.z = i * 0.4;
  scene.add(ring);
  feRings.push(ring);
}

// Layer 8 -- Mechanotransduction overlay
let showLayer8 = true;
// Stress halo: sphere at each joint, radius/color by stress
const mechHalos = [];
const STRESS_LOW  = 0x437A22;  // green -- below integrin threshold
const STRESS_MID  = 0xE8AF34;  // amber -- integrin active
const STRESS_HIGH = 0xA13544;  // red   -- YAP/TAZ active
for(let k=0; k<12; k++){
  const geo = new THREE.SphereGeometry(0.04, 8, 6);
  const mat = new THREE.MeshBasicMaterial({
    color: STRESS_LOW, transparent: true, opacity: 0.0,
    blending: THREE.AdditiveBlending, depthWrite: false
  });
  const halo = new THREE.Mesh(geo, mat);
  halo.visible = false;
  scene.add(halo);
  mechHalos.push(halo);
}
// Piezo flash: point light per joint (only one active at a time)
const piezoLight = new THREE.PointLight(0xBCE2E7, 0, 1.2);
scene.add(piezoLight);
// Remodel ring: torus at dominant zone
const remodelGeo = new THREE.TorusGeometry(0.09, 0.008, 6, 24);
const remodelMat = new THREE.MeshBasicMaterial({
  color: 0xD163A7, transparent: true, opacity: 0.0,
  blending: THREE.AdditiveBlending, depthWrite: false
});
const remodelRing = new THREE.Mesh(remodelGeo, remodelMat);
scene.add(remodelRing);

// Layer 9 -- Neuro-fascial integration
let showLayer9 = true;
// Afferent pulse lines -- one per bone segment
const PULSE_SEGS = [[0,1],[1,2],[2,3],[3,8],[8,9],[9,10],[10,11],[3,4],[4,5],[8,7],[7,6]];
const pulseLines = [];
for(let s=0; s<PULSE_SEGS.length; s++){
  const geo = new THREE.BufferGeometry();
  const arr = new Float32Array(6);
  geo.setAttribute("position", new THREE.BufferAttribute(arr,3));
  const mat = new THREE.LineBasicMaterial({
    color: 0xBCE2E7, transparent: true, opacity: 0.0,
    blending: THREE.AdditiveBlending, depthWrite: false
  });
  const line = new THREE.Line(geo, mat);
  scene.add(line);
  pulseLines.push(line);
}
// Autonomic tone bar -- rendered as a small screen-space div updated via DOM
// Receptor type indicator -- DOM only
// Ruffini glow: yellow-green dots at sustained-load joints
const ruffiniDots = [];
for(let k=0; k<12; k++){
  const geo = new THREE.SphereGeometry(0.025, 6, 4);
  const mat = new THREE.MeshBasicMaterial({
    color: 0x6DAA45, transparent: true, opacity: 0.0,
    blending: THREE.AdditiveBlending, depthWrite: false
  });
  const dot = new THREE.Mesh(geo, mat);
  dot.visible = false;
  scene.add(dot);
  ruffiniDots.push(dot);
}

// Layer 10 -- 12-Channel peripheral nerve overlay
let showLayer10 = true;
const CH_COLORS = {
  LU:"#BCE2E7", LI:"#FFC553", ST:"#E8AF34", SP:"#FFC553",
  HT:"#DD6974", SI:"#A13544", BL:"#5591C7", KD:"#006494",
  PC:"#D163A7", TW:"#A86FDF", GB:"#6DAA45", LV:"#437A22"
};
// Channel segment pairs mapped to joint index pairs
// 12 joints: 0=R_ank,1=R_kne,2=R_hip,3=R_sho,4=R_elb,5=R_wri
//            6=L_wri,7=L_elb,8=L_sho,9=L_hip,10=L_kne,11=L_ank
const CH_SEGS = {
  LU:[[8,7],[7,6]],   LI:[[3,4],[4,5]],
  ST:[[11,10],[10,9],[9,3]],  SP:[[11,10],[10,9]],
  HT:[[8,7],[7,6]],   SI:[[3,4],[4,5]],
  BL:[[11,10],[10,9],[9,3],[3,8]], KD:[[11,10],[10,9],[9,8]],
  PC:[[8,7],[7,6]],   TW:[[3,4],[4,5]],
  GB:[[11,10],[10,9],[9,3]], LV:[[11,10],[10,9]]
};
const chLines = {};
Object.keys(CH_SEGS).forEach(ch=>{
  chLines[ch] = CH_SEGS[ch].map(([a,b])=>{
    const geo = new THREE.BufferGeometry();
    const arr = new Float32Array(6);
    geo.setAttribute("position", new THREE.BufferAttribute(arr,3));
    const col = parseInt(CH_COLORS[ch].replace("#",""), 16);
    const mat = new THREE.LineBasicMaterial({
      color: col, transparent: true, opacity: 0.0,
      blending: THREE.AdditiveBlending, depthWrite: false
    });
    const line = new THREE.Line(geo, mat);
    scene.add(line);
    return {line, a, b};
  });
});
// -----------------------------------------------------------
// State
// -----------------------------------------------------------
let currentFrame = null;
let playing = true;
let totalFrames = 240;
let currentIdx = 0;

function hexToRgb(hex){
  const r=parseInt(hex.slice(1,3),16)/255;
  const g=parseInt(hex.slice(3,5),16)/255;
  const b=parseInt(hex.slice(5,7),16)/255;
  return [r,g,b];
}

function applyFrame(data){
  if(!data || !data.joints) return;
  const jpos = data.joints.map(j => new THREE.Vector3(j.x, j.y, j.z));
  currentFrame = data;
  const jd = data.joints;
  for(let j=0; j<Math.min(jd.length, JOINT_COUNT); j++){
    const jt = jd[j];
    const pos = new THREE.Vector3(jt.x, jt.y, jt.z);
    joints[j].position.copy(pos);
    const c = new THREE.Color(jt.rgb[0], jt.rgb[1], jt.rgb[2]);
    joints[j].material.color.copy(c);
    joints[j].material.emissive.copy(c);
    joints[j].material.emissiveIntensity = 0.3 + jt.A * 0.7;
    glows[j].position.copy(pos);
    glows[j].material.color.copy(c);
    glows[j].material.opacity = 0.12 + jt.A * 0.35;
  }
  boneLines.forEach(({geo,a,b})=>{
    const A = data.joints[a], B = data.joints[b];
    if(!A||!B) return;
    const pos = geo.attributes.position;
    pos.setXYZ(0, A.x, A.y, A.z);
    pos.setXYZ(1, B.x, B.y, B.z);
    pos.needsUpdate = true;
    const col = geo.attributes.color;
    col.setXYZ(0, A.rgb[0], A.rgb[1], A.rgb[2]);
    col.setXYZ(1, B.rgb[0], B.rgb[1], B.rgb[2]);
    col.needsUpdate = true;
  });
  // Layer 2 -- ICR arrows
  for(let j=0; j<Math.min(jd.length, JOINT_COUNT); j++){
    const jt = jd[j];
    const icr = jt.icr;
    const origin = new THREE.Vector3(jt.x, jt.y, jt.z);
    if(icr && icr.valid && showLayer2){
      const cpDir = new THREE.Vector3(icr.cp[0], icr.cp[1], icr.cp[2]).normalize();
      const cfDir = new THREE.Vector3(icr.cf[0], icr.cf[1], icr.cf[2]).normalize();
      const len = ARROW_SCALE * (0.5 + icr.lam * 0.8);
      arrowsCP[j].position.copy(origin);
      arrowsCP[j].setDirection(cpDir);
      arrowsCP[j].setLength(len, len*0.22, len*0.12);
      arrowsCP[j].visible = true;
      arrowsCF[j].position.copy(origin);
      arrowsCF[j].setDirection(cfDir);
      arrowsCF[j].setLength(len, len*0.22, len*0.12);
      arrowsCF[j].visible = true;
    } else {
      arrowsCP[j].visible = false;
      arrowsCF[j].visible = false;
    }
  }
  document.getElementById("frame-num").textContent = data.frame;
  document.getElementById("global-A").textContent = data.global_A.toFixed(3);
  // Layer 10 channel nerve overlay
  if(data.channel && showLayer10){
    const ch = data.channel;
    Object.keys(chLines).forEach(key=>{
      const act = (ch.activation[key] || 0);
      chLines[key].forEach(({line, a, b})=>{
        const pa = jpos[a] || new THREE.Vector3();
        const pb = jpos[b] || new THREE.Vector3();
        const pos = line.geometry.attributes.position;
        pos.setXYZ(0, pa.x, pa.y, pa.z);
        pos.setXYZ(1, pb.x, pb.y, pb.z);
        pos.needsUpdate = true;
        line.material.opacity = act * 0.75;
      });
    });
    // DOM panel updates
    const dc = document.getElementById("ch-dominant");
    if(dc) dc.textContent = (ch.dominant_channel || "-") + " (" + (ch.dominant_element || "-") + ")";
    const cy = document.getElementById("ch-yin");
    if(cy) cy.textContent = Math.round((ch.yin_total||0)*100) + "%";
    const cya = document.getElementById("ch-yang");
    if(cya) cya.textContent = Math.round((ch.yang_total||0)*100) + "%";
    // Element load bars
    ["Metal","Earth","Fire","Water","Wood"].forEach(el=>{
      const bar = document.getElementById("el-bar-"+el);
      if(bar) bar.style.width = Math.round((ch.element_load[el]||0)*80) + "px";
      const asym = document.getElementById("el-asym-"+el);
      if(asym){
        const v = ch.asymmetry[el] || 0;
        asym.textContent = (v >= 0 ? "Yang+" : "Yin+") + Math.abs(Math.round(v*100)) + "%";
        asym.style.color = v >= 0 ? "#DD6974" : "#5591C7";
      }
    });
  } else if(!showLayer10){
    Object.values(chLines).forEach(segs=>segs.forEach(({line})=>line.material.opacity=0));
  }
  // Layer 9 neuro-fascial
  if(data.neuro && showLayer9){
    const n = data.neuro;
    // Afferent pulse along bone segments
    PULSE_SEGS.forEach(([a,b], si)=>{
      const pa = jpos[a] || new THREE.Vector3();
      const pb = jpos[b] || new THREE.Vector3();
      const avgPulse = ((n.pulse[a]||0) + (n.pulse[b]||0)) * 0.5;
      const pos = pulseLines[si].geometry.attributes.position;
      pos.setXYZ(0, pa.x, pa.y, pa.z);
      pos.setXYZ(1, pb.x, pb.y, pb.z);
      pos.needsUpdate = true;
      pulseLines[si].material.opacity = showLayer9 ? avgPulse * 0.8 : 0.0;
    });
    // Ruffini glow at joints with sustained load
    for(let k=0; k<12; k++){
      const p = jpos[k] || new THREE.Vector3();
      ruffiniDots[k].position.copy(p);
      ruffiniDots[k].material.opacity = showLayer9 ? (n.ruffini[k]||0) * 0.7 : 0.0;
      ruffiniDots[k].visible = true;
    }
    // DOM updates
    const rec = document.getElementById("neuro-receptor");
    if(rec) rec.textContent = n.receptor || "-";
    const aut = document.getElementById("neuro-autonomic");
    if(aut){
      const pct = Math.round(n.autonomic * 100);
      aut.textContent = pct + "% SNS / " + (100-pct) + "% PNS";
      aut.style.color = n.autonomic > 0.6 ? "#A13544" : "#437A22";
    }
    const prop = document.getElementById("neuro-prop");
    if(prop) prop.textContent = Math.round(n.prop * 100) + "%";
  } else if(!showLayer9){
    pulseLines.forEach(l=>l.material.opacity=0);
    ruffiniDots.forEach(d=>d.visible=false);
  }
  // Layer 8 mechanotransduction
  if(data.mech && showLayer8){
    const m = data.mech;
    let maxPiezo = 0; let maxPiezoPos = null;
    for(let k=0; k<12; k++){
      const p = jpos[k] || new THREE.Vector3();
      const s = m.stress[k] || 0;
      const pz = m.piezo[k] || 0;
      const halo = mechHalos[k];
      halo.position.copy(p);
      // Scale halo by stress
      const sc = 0.04 + s * 0.10;
      halo.scale.setScalar(sc / 0.04);
      // Color by threshold
      if(m.yap_taz[k])       halo.material.color.setHex(STRESS_HIGH);
      else if(m.integrin[k]) halo.material.color.setHex(STRESS_MID);
      else                   halo.material.color.setHex(STRESS_LOW);
      halo.material.opacity = 0.10 + s * 0.55;
      halo.visible = true;
      // Track max piezo for point light
      if(pz > maxPiezo){ maxPiezo = pz; maxPiezoPos = p; }
    }
    // Piezo point light at highest piezo joint
    if(maxPiezoPos){
      piezoLight.position.copy(maxPiezoPos);
      piezoLight.intensity = maxPiezo * 1.2;
    }
    // Remodel ring at dominant zone
    const dz = m.dominant_zone || 0;
    const dp = jpos[dz] || new THREE.Vector3();
    remodelRing.position.copy(dp);
    remodelRing.rotation.x += 0.05;
    const hasRemodel = m.remodel && m.remodel[dz];
    remodelMat.opacity = hasRemodel ? 0.7 : (m.stress[dz] > 0.5 ? 0.15 : 0.0);
    remodelRing.visible = true;
    // Mech index in HUD
    const mi = document.getElementById("mech-index");
    if(mi) mi.textContent = Math.round(m.mech_index * 100) + "%";
  } else if(!showLayer8){
    mechHalos.forEach(h=>h.visible=false);
    remodelRing.visible=false;
    piezoLight.intensity=0;
  }
  // Layer 7 Ba Gang
  if(data.ba_gang){
    const bg = data.ba_gang;
    const p = document.getElementById("bg-pattern");
    if(p) p.textContent = bg.pattern || "-";
    const bar = document.getElementById("bg-yin-bar");
    if(bar) bar.style.width = Math.round(bg.yin * 120) + "px";
    const ie = document.getElementById("bg-ie");
    if(ie){ ie.textContent = Math.round(bg.interior*100) + "% Int / " + Math.round(bg.exterior*100) + "% Ext";
      ie.style.color = bg.interior > 0.5 ? "#A13544" : "#4F98A3"; }
    const ch = document.getElementById("bg-ch");
    if(ch){ ch.textContent = Math.round(bg.cold*100) + "% Cold / " + Math.round(bg.hot*100) + "% Hot";
      ch.style.color = bg.cold > 0.5 ? "#5591C7" : "#A13544"; }
    const de = document.getElementById("bg-de");
    if(de){ de.textContent = Math.round(bg.deficient*100) + "% Def / " + Math.round(bg.excess*100) + "% Exc";
      de.style.color = bg.deficient > 0.5 ? "#7A7974" : "#E8AF34"; }
    const cf = document.getElementById("bg-conf");
    if(cf) cf.textContent = Math.round(bg.confidence*100) + "%";
  }
  document.getElementById("scrubber").value = data.frame;
  currentIdx = data.frame;
}

// -----------------------------------------------------------
// WebSocket
// -----------------------------------------------------------
let ws;
let wsInterval = null;
function connectWS(){
  ws = new WebSocket("ws://"+location.host+"/ws");
  ws.onopen = ()=>{
    document.getElementById("hud-status").textContent = "LIVE";
    document.getElementById("hud-status").className = "status-live";
  };
  ws.onmessage = e=>{
    const d = JSON.parse(e.data);
    if(d.type==="init"){
      totalFrames = d.total;
      document.getElementById("frame-total").textContent = d.total;
      document.getElementById("scrubber").max = d.total-1;
    } else if(d.type==="frame"){
      applyFrame(d);
    }
  };
  ws.onclose = ()=>{
    clearInterval(wsInterval);
    document.getElementById("hud-status").textContent = "RECONNECTING...";
    document.getElementById("hud-status").className = "status-conn";
    setTimeout(connectWS, 1500);
  };
}
connectWS();

document.getElementById("chk-L1").onchange = e=>{
  joints.forEach(j=>j.visible=e.target.checked);
  glows.forEach(g=>g.visible=e.target.checked);
  boneLines.forEach(b=>b.line.visible=e.target.checked);
};
document.getElementById("chk-L2").onchange = e=>{
  showLayer2 = e.target.checked;
  if(!showLayer2){
    arrowsCP.forEach(a=>a.visible=false);
    arrowsCF.forEach(a=>a.visible=false);
  }
};

// -----------------------------------------------------------
// Controls
// -----------------------------------------------------------
document.getElementById("btn-play").onclick  = ()=>{ playing=true; if(ws&&ws.readyState===1) ws.send(JSON.stringify({cmd:"play"})); };
document.getElementById("btn-pause").onclick = ()=>{ playing=false; if(ws&&ws.readyState===1) ws.send(JSON.stringify({cmd:"pause"})); };
document.getElementById("btn-reset").onclick = ()=>{
  playing=false;
  if(ws&&ws.readyState===1) ws.send(JSON.stringify({cmd:"seek",frame:0}));
  if(ws&&ws.readyState===1) ws.send(JSON.stringify({cmd:"play"}));
};
document.getElementById("scrubber").oninput = e=>{
  playing=false;
  if(ws&&ws.readyState===1) ws.send(JSON.stringify({cmd:"seek",frame:parseInt(e.target.value)}));
};

// -----------------------------------------------------------
// Render loop
// -----------------------------------------------------------
(function animate(){
  requestAnimationFrame(animate);
  renderer.render(scene, camera);
})();
