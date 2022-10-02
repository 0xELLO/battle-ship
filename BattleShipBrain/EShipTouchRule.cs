using System.ComponentModel.DataAnnotations;

namespace BattleShipBrain
{
    public enum EShipTouchRule
    {
        [Display(Name ="No touch rule")]
        NoTouch = 1,
        [Display(Name ="Corner touch rule")]
        CornerTouch = 2,
        [Display(Name ="Side touch rule")]
        SideTouch = 3
    }
}