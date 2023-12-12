using System;

namespace CycladeUI.Test
{
    public enum LogType
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal,
    }
    
    [Serializable]
    public class TestModel
    {
        public LogType logLevel = LogType.Trace;

        public bool isUdp = true;
        public bool connectWithoutServiceDiscovery = true;
        public string localServiceAddress;
        public string discoveryAddress;
        public string overrideUniqueId;
        public OfflineClientOptions offline;
        public NetworkSettings network = new();
    }
    
    [Serializable]
    public class AnotherModel
    {
        public long longValue;
    }
    
    [Serializable]
    public class OfflineClientOptions
    {
        public bool available;
        public int OfflineRetrySeconds = 10;
    }
    
    [Serializable]
    public class NetworkSettings
    {
        public bool udpLatency;
        public int udpMinLatencyMs = 30;
        public int udpMaxLatencyMs = 100;

        public bool udpPacketLoss;
        public int udpPacketLossChance = 10;
        public Deep1 deep1 = new();
    }

    [Serializable]
    public class Deep1
    {
        public ushort testUshort;
        public Deep2 deep2 = new();
    }
    
    [Serializable]
    public class Deep2
    {
        public ushort testUshort;
    } 
    
    [Serializable]
    public class Player
    {
        public bool canJump;
        public bool canMove;
        public bool canAttack;
        public AttackModel attackModel = new();
    }
    
    public enum TypeOfAttack
    {
        Melee,
        Range
    }
    
    [Serializable]
    public class AttackModel
    {
        public TypeOfAttack type;
        public int range;
        public float strength;
    }
    
    
}