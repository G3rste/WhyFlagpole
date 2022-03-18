using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace CFlag
{
    public class BlockBehaviorPole : BlockBehavior
    {
        public BlockBehaviorPole(Block block) : base(block)
        {
        }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            var byEntity = byPlayer.Entity;
            if (blockSel != null && byPlayer.Entity.Controls.Sprint)
            {
                var blockAccessor = world.BlockAccessor;
                var upPos = blockSel.Position.Copy();
                while(blockAccessor.GetBlock(upPos).HasBehavior<BlockBehaviorPole>()){
                    upPos = upPos.Up();
                    if(blockAccessor.GetBlock(upPos).HasBehavior<BlockBehaviorFlag>()){
                        world.RegisterCallbackUnique(tryFlipFlagDownwards, upPos, 500);
                    }
                }
            }
            return base.OnBlockInteractStart(world, byPlayer, blockSel, ref handling);
        }

        private void tryFlipFlagDownwards(IWorldAccessor worldAccessor, BlockPos pos, float dt)
        {
            IBlockAccessor blockAccessor = worldAccessor.BlockAccessor;
            var downPos = pos.DownCopy();
            var flag = blockAccessor.GetBlock(pos);
            var pole = blockAccessor.GetBlock(downPos);
            if (pole.HasBehavior<BlockBehaviorPole>() && flag.HasBehavior<BlockBehaviorFlag>())
            {
                blockAccessor.ExchangeBlock(pole.Id, pos);
                blockAccessor.ExchangeBlock(flag.Id, downPos);
                worldAccessor.RegisterCallbackUnique(tryFlipFlagDownwards, downPos, 500);
            }
        }

        // public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer, ref EnumHandling handling)
        // {
        //     handling = EnumHandling.PassThrough;
        //     return new WorldInteraction[]
        //     {
        //         new WorldInteraction()
        //         {
        //             ActionLangCode = "cflag-pole-interact",
        //             HotKeyCode = "sprint",
        //             MouseButton = EnumMouseButton.Right
        //         },
        //         new WorldInteraction()
        //         {
        //             ActionLangCode = "cflag-pole-add",
        //             HotKeyCode = null,
        //             MouseButton = EnumMouseButton.Right
        //         }
        //     };
        // }
    }
}