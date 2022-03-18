using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace CFlag
{
    public class BlockBehaviorFlag : BlockBehavior
    {
        public BlockBehaviorFlag(Block block) : base(block)
        {
        }
        public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref EnumHandling handling, ref string failureCode)
        {
            handling = EnumHandling.PreventDefault;
            BlockPos pos = blockSel.Position.AddCopy(blockSel.Face.Opposite);
            Block attachingBlock = world.BlockAccessor.GetBlock(pos);
            if (attachingBlock.HasBehavior<BlockBehaviorPole>())
            {
                // Might need a little extra logic for handling different flag facing
                world.BlockAccessor.ExchangeBlock(block.Id, pos);
                return true;
            }
            else
            {
                failureCode = "flagneedspole";
                return false;
            }
        }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            var byEntity = byPlayer.Entity;
            if (blockSel != null && byPlayer.Entity.Controls.Sprint)
            {
                byEntity.World.RegisterCallbackUnique(tryFlipFlagUpwards, blockSel.Position, 500);
            }
            return base.OnBlockInteractStart(world, byPlayer, blockSel, ref handling);
        }

        private void tryFlipFlagUpwards(IWorldAccessor worldAccessor, BlockPos pos, float dt)
        {
            IBlockAccessor blockAccessor = worldAccessor.BlockAccessor;
            var upPos = pos.UpCopy();
            var flag = blockAccessor.GetBlock(pos);
            var pole = blockAccessor.GetBlock(upPos);
            if (pole.HasBehavior<BlockBehaviorPole>() && flag.HasBehavior<BlockBehaviorFlag>())
            {
                blockAccessor.ExchangeBlock(pole.Id, pos);
                blockAccessor.ExchangeBlock(flag.Id, upPos);
                worldAccessor.RegisterCallbackUnique(tryFlipFlagUpwards, upPos, 500);
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