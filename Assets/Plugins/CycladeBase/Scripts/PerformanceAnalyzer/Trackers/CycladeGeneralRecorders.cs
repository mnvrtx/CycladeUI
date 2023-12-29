using CycladeBase.PerformanceAnalyzer.Trackers.Base;
using CycladeBase.PerformanceAnalyzer.Trackers.Base.Recorders;
using CycladeBase.Utils;

namespace CycladeBase.PerformanceAnalyzer.Trackers
{
    public class CycladeGeneralRecorders : BaseRecordersTracker<CycladeGeneralRecorders>
    {
        public static long DefaultNormalTrianglesCount = 50_000;
        public static long DefaultCriticalTrianglesCount = 200_000;
        
        public static long DefaultNormalBatchesCount = 500;
        public static long DefaultCriticalBatchesCount = 1000;
        
        public static long DefaultNormalMemoryMB = 
#if UNITY_EDITOR
            10_000;
#else
            1300;
#endif
        
        public static long DefaultCriticalMemoryMB =
#if UNITY_EDITOR
            13_000;
#else
            1800;
#endif
            
        
        
        public static float DefaultNormalGCMemory = 4;
        public static float DefaultCriticalGCMemory = 8;
        
        public static float DefaultNormalThreadMs = 7.5f;
        public static float DefaultCriticalThreadMs = 15;
        
        public static float DefaultNormalFpsMs = 25;
        public static float DefaultCriticalFpsMs = 15;
        
        protected override void Awake()
        {
            RegisterRecorder(new FpsRecorder(), new ValSuffix((float v) => $"{v:F2} FPS")).SetCustomThresholds(DefaultNormalFpsMs, DefaultCriticalFpsMs, true);
            RegisterRecorder(new MainThreadRecorder(), new ValSuffix((float v) => $"{v:F2}ms")).SetCustomThresholds(DefaultCriticalThreadMs, DefaultNormalThreadMs);
            RegisterRecorder(new BatchesRecorder(), new ValSuffix((long v) => $"{v:N0}")).SetCustomThresholds(DefaultCriticalBatchesCount, DefaultNormalBatchesCount);
            RegisterRecorder(new TrianglesRecorder(), new ValSuffix((long v) => $"{v:N0}")).SetCustomThresholds(DefaultCriticalTrianglesCount, DefaultNormalTrianglesCount);
            RegisterRecorder(new UsedMemoryRecorder(), new ValSuffix((long v) => $"{v:N0} MB")).SetCustomThresholds(DefaultCriticalMemoryMB, DefaultNormalMemoryMB);
            RegisterRecorder(new FrameGCMemoryRecorder(), new ValSuffix((float val) => ((long)(val * 1024)).SizeSuffix())).SetCustomThresholds(DefaultCriticalGCMemory, DefaultNormalGCMemory);
        }
    }
}