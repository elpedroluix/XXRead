// Pull-to-refresh on a page, with pulltorefresh.js (MIT, embedded in wwwroot/lib)
import PullToRefresh from '../lib/pulltorefreshjs/index.esm.js';

// Returns the pulltorefresh instance : the component calls its destroy() when it is disposed
export function init(element, dotNetComponent, texts) {
    return PullToRefresh.init({
        mainElement: element,
        triggerElement: element,
        instructionsPullToRefresh: texts.pull,
        instructionsReleaseToRefresh: texts.release,
        instructionsRefreshing: texts.refreshing,
        // The indicator stays until the .NET refresh is done
        onRefresh: () => dotNetComponent.invokeMethodAsync('OnPullToRefresh')
    });
}
