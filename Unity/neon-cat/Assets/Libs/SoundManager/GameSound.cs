namespace LeakyAbstraction
{
    // todo: codegen om import
    // This enum defines the sound types available to play.
    // Each enum value can have AudioClips assigned to it in the SoundManager's Inspector pane.
    public enum GameSound
    {
        // It's advisable to keep 'None' as the first option, since it helps exposing this enum in the Inspector.
        // If the first option is already an actual value, then there is no "nothing selected" option.
        None,
        SfxTimeFreezeEnter,
        SfxTimeFreezeExit,
        SfxShipJump,
        SfxPickupCoin,
        SfxShipHitObstacle,
        SfxShatterHitGround,
        SfxBlockCrack,
        SfxBlockExplode,
        SfxTeleportOpen,
        SfxTeleportClose,
        SfxShipCharge,
        SfxSurfaceDischarge,
        SfxButtonRegularTap,
        SfxLevelComplete,
        SfxDieByFalling
    }
}