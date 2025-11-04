using TS.PageSlider;
using UnityEngine;

namespace SkewerJam.Utils.PageSliderPack
{
    public class CustomPageSlider : PageSlider
    {

        public void SetImmediatePage(int index)
        {
            _scroller.SetImmediatePage(index);
        }


        public void ScrollToPreviousPage()
        {
            var prevPage = Mathf.Max(_scroller.CurrentPage - 1, 0);
            _scroller.ScrollToPage(prevPage);
        }

        public void ScrollToNextPage()
        {
            var nextPage = Mathf.Min(_scroller.CurrentPage + 1, _scroller.GetPageCount());
            _scroller.ScrollToPage(nextPage);
        }
    }
}