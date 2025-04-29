using UntitledGooseMod.Defs;
using Verse;

namespace UntitledGooseMod.MapComponents
{
    public class MapComponent_MischievousGooseArrival : MapComponent
    {
        private int _spawnInterval = 60;
        private int _nextSpawnTick;
        private int _remainingGeese;
        private IntVec3 _spawnCenter;
        private PawnKindDef _gooseKind;
        private int _exitAfterTicks;
        
        public MapComponent_MischievousGooseArrival(Map map) : base(map)
        {
            
        }
        
        public void StartSpawningGeese(int numGeese, IntVec3 center, 
            PawnKindDef kind, int exitAfterTicks)
        {
            _remainingGeese = numGeese;
            _spawnCenter = center;
            _gooseKind = kind;
            _exitAfterTicks = exitAfterTicks;
            _nextSpawnTick = Find.TickManager.TicksGame + _spawnInterval;
        }
        
        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (_remainingGeese <= 0) return;
            if (Find.TickManager.TicksGame < _nextSpawnTick) return;
            
            IntVec3 spawnLoc = CellFinder.RandomClosewalkCellNear(
                _spawnCenter, map, 10);
            Pawn goose = PawnGenerator.GeneratePawn(_gooseKind);
            GenSpawn.Spawn(goose, spawnLoc, map, Rot4.Random);
            goose.mindState.exitMapAfterTick = Find.TickManager.TicksGame + _exitAfterTicks;
            
            if (goose.mindState is { mentalStateHandler: not null })
            {
                goose.mindState.mentalStateHandler
                    .TryStartMentalState(UGMDefOf.UGM_MischievousGoose, null, true);
            }
            
            _remainingGeese--;
            _nextSpawnTick = Find.TickManager.TicksGame + _spawnInterval;
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _spawnInterval, "_spawnInterval", 60);
            Scribe_Values.Look(ref _nextSpawnTick, "_nextSpawnTick", 0);
            Scribe_Values.Look(ref _remainingGeese, "_remainingGeese", 0);
            Scribe_Values.Look(ref _exitAfterTicks, "_exitAfterTicks", 0);
            Scribe_Values.Look(ref _spawnCenter, "_spawnCenter", IntVec3.Invalid);
            Scribe_Defs.Look(ref _gooseKind, "_gooseKind");
        }
    }
}