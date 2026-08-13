using System.Collections.Generic;

public enum EducationalEvidenceClass
{
    RigProxy,
    BiomedicalReference,
    ModelDerived,
    TraditionalFramework,
    ResearchHypothesis
}

public sealed class AnatomicalCrosswalkEntry
{
    public string RigBoneName { get; }
    public string EducationalLabel { get; }
    public string Region { get; }
    public string Notes { get; }
    public EducationalEvidenceClass EvidenceClass { get; }

    public AnatomicalCrosswalkEntry(
        string rigBoneName,
        string educationalLabel,
        string region,
        string notes,
        EducationalEvidenceClass evidenceClass = EducationalEvidenceClass.RigProxy)
    {
        RigBoneName = rigBoneName;
        EducationalLabel = educationalLabel;
        Region = region;
        Notes = notes;
        EvidenceClass = evidenceClass;
    }
}

public static class AnatomicalCrosswalk
{
    /*
     * This maps technical animation-rig bone identifiers to conservative
     * educational terminology. It does not assert that a rig bone is a
     * complete anatomical structure or a patient-specific representation.
     */
    public static readonly IReadOnlyDictionary<string, AnatomicalCrosswalkEntry> ByRigBone =
        new Dictionary<string, AnatomicalCrosswalkEntry>
        {
            ["spine"] = new(
                "spine",
                "Axial spine root proxy",
                "Axial skeleton",
                "Animation-rig root for the axial chain; not assigned to a specific vertebral level."),

            ["spine.001"] = new(
                "spine.001",
                "Lower axial spine proxy",
                "Axial skeleton",
                "Represents a lower portion of the avatar axial chain; not a named lumbar vertebra."),

            ["spine.002"] = new(
                "spine.002",
                "Mid axial spine proxy",
                "Axial skeleton",
                "Represents a mid portion of the avatar axial chain; not a named thoracic vertebra."),

            ["spine.003"] = new(
                "spine.003",
                "Upper axial / shoulder-girdle attachment proxy",
                "Axial skeleton",
                "Parent of both shoulder rig anchors; not a discrete vertebra, clavicle, or scapula."),

            ["spine.004"] = new(
                "spine.004",
                "Upper axial spine proxy",
                "Axial skeleton",
                "Rig segment only; this asset has no separately named cervical vertebrae."),

            ["spine.005"] = new(
                "spine.005",
                "Superior axial spine terminus proxy",
                "Axial skeleton",
                "Rig segment only; this asset has no skull, neck, or head bone."),

            ["pelvis.L"] = new(
                "pelvis.L",
                "Left pelvic / hip-region anchor",
                "Pelvic girdle",
                "Rig anchor; not a separate ilium, ischium, pubis, acetabulum, or sacrum."),

            ["pelvis.R"] = new(
                "pelvis.R",
                "Right pelvic / hip-region anchor",
                "Pelvic girdle",
                "Rig anchor; not a separate ilium, ischium, pubis, acetabulum, or sacrum."),

            ["thigh.L"] = new(
                "thigh.L",
                "Left femoral segment",
                "Left lower limb",
                "Animation segment approximating the femur; does not itself define the hip or knee joint."),

            ["thigh.R"] = new(
                "thigh.R",
                "Right femoral segment",
                "Right lower limb",
                "Animation segment approximating the femur; does not itself define the hip or knee joint."),

            ["shin.L"] = new(
                "shin.L",
                "Left tibial-fibular segment",
                "Left lower limb",
                "Single rig segment for the lower leg; does not separately represent tibia and fibula."),

            ["shin.R"] = new(
                "shin.R",
                "Right tibial-fibular segment",
                "Right lower limb",
                "Single rig segment for the lower leg; does not separately represent tibia and fibula."),

            ["foot.L"] = new(
                "foot.L",
                "Left ankle-foot segment",
                "Left lower limb",
                "Rig segment; not separate tarsal, metatarsal, or ankle-joint anatomy."),

            ["foot.R"] = new(
                "foot.R",
                "Right ankle-foot segment",
                "Right lower limb",
                "Rig segment; not separate tarsal, metatarsal, or ankle-joint anatomy."),

            ["toe.L"] = new(
                "toe.L",
                "Left forefoot / toe segment",
                "Left lower limb",
                "Rig segment; not individual phalanges."),

            ["toe.R"] = new(
                "toe.R",
                "Right forefoot / toe segment",
                "Right lower limb",
                "Rig segment; not individual phalanges."),

            ["heel.02.L"] = new(
                "heel.02.L",
                "Left calcaneal-region anchor",
                "Left lower limb",
                "Rig anchor near the heel; not a geometric model of the calcaneus."),

            ["heel.02.R"] = new(
                "heel.02.R",
                "Right calcaneal-region anchor",
                "Right lower limb",
                "Rig anchor near the heel; not a geometric model of the calcaneus."),

            ["shoulder.L"] = new(
                "shoulder.L",
                "Left shoulder-girdle anchor",
                "Left upper limb",
                "Rig anchor; not separately a clavicle, scapula, or glenohumeral joint."),

            ["shoulder.R"] = new(
                "shoulder.R",
                "Right shoulder-girdle anchor",
                "Right upper limb",
                "Rig anchor; not separately a clavicle, scapula, or glenohumeral joint."),

            ["upper_arm.L"] = new(
                "upper_arm.L",
                "Left humeral segment",
                "Left upper limb",
                "Animation segment approximating the humerus."),

            ["upper_arm.R"] = new(
                "upper_arm.R",
                "Right humeral segment",
                "Right upper limb",
                "Animation segment approximating the humerus."),

            ["forearm.L"] = new(
                "forearm.L",
                "Left radius-ulna segment",
                "Left upper limb",
                "Single rig segment; does not separately represent radius and ulna."),

            ["forearm.R"] = new(
                "forearm.R",
                "Right radius-ulna segment",
                "Right upper limb",
                "Single rig segment; does not separately represent radius and ulna."),

            ["hand.L"] = new(
                "hand.L",
                "Left wrist-hand segment",
                "Left upper limb",
                "Rig segment; not a separate wrist-joint or carpal model."),

            ["hand.R"] = new(
                "hand.R",
                "Right wrist-hand segment",
                "Right upper limb",
                "Rig segment; not a separate wrist-joint or carpal model."),
        };

    public static bool TryGet(string rigBoneName, out AnatomicalCrosswalkEntry entry)
    {
        return ByRigBone.TryGetValue(rigBoneName, out entry);
    }
}
