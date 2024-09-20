using JumpKing.Mods;
#if DEBUG
using ConveyorBlockMod.Utils;
#endif
using JumpKing.Level;
using EntityComponent;
using JumpKing.Player;
using ConveyorBlockMod.Blocks;

namespace ConveyorBlockMod
{
    [JumpKingMod("Mc__Ouille.ConveyorBlockMod")]
    public static class ModEntry
    {
        /// <summary>
        /// Called by Jump King before the level loads
        /// </summary>
        [BeforeLevelLoad]
        public static void BeforeLevelLoad()
        {
#if DEBUG
            Logger.EraseAll();
            Logger.StartTimer();
            Logger.Log("ConveyorBlockMod loaded");
#endif
            LevelManager.RegisterBlockFactory(new ConveyorBlockFactory());
        }

        /// <summary>
        /// Called by Jump King when the level unloads
        /// </summary>
        [OnLevelUnload]
        public static void OnLevelUnload()
        {
        }

        /// <summary>
        /// Called by Jump King when the Level Starts
        /// </summary>
        [OnLevelStart]
        public static void OnLevelStart()
        {
            var player = EntityManager.instance.Find<PlayerEntity>();
            player?.m_body?.RegisterBlockBehaviour(typeof(ConveyorBlock), new ConveyorBlockBehaviour());
        }

        /// <summary>
        /// Called by Jump King when the Level Ends
        /// </summary>
        [OnLevelEnd]
        public static void OnLevelEnd()
        {
        }
    }
}
