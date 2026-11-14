// Scroll position of the stories list, saved when a story is opened (MudBlazor's IScrollManager has no getter)
export function getScrollPosition() {
    return window.scrollY;
}
