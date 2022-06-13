using CefSharp;
using CefSharp.Wpf;
using Netbird.browser.handlers;
using System;
using System.IO;
using System.Windows;

namespace Netbird.browser
{
    public class NetbirdChromium : ChromiumWebBrowser
    {

        private TabController tabController;

        public NetbirdChromium(TabController tabController)
        {
            BrowserSettings.WindowlessFrameRate = 120;
            
            updateController(tabController);
            

      

            LoadingStateChanged += (sender, args) =>
            {
                BrowserCore.ExecuteScriptAsync(smoothScroll);
                //Wait for the Page to finish loading
                if (args.IsLoading == true)
                {
                    if (tabController == null) return;
                    tabController.window.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                       
                          //  loadExtention();
                        }
                        catch { }
                    });





                }
            };


        }

        
        String smoothScroll = "(function () {\n" +
         "  \n" +
         "// Scroll Variables (tweakable)\n" +
         "var defaultOptions = {\n" +
         "\n" +
         "    // Scrolling Core\n" +
         "    frameRate        : 150, // [Hz]\n" +
         "    animationTime    : 400, // [ms]\n" +
         "    stepSize         : 100, // [px]\n" +
         "\n" +
         "    // Pulse (less tweakable)\n" +
         "    // ratio of \"tail\" to \"acceleration\"\n" +
         "    pulseAlgorithm   : true,\n" +
         "    pulseScale       : 4,\n" +
         "    pulseNormalize   : 1,\n" +
         "\n" +
         "    // Acceleration\n" +
         "    accelerationDelta : 50,  // 50\n" +
         "    accelerationMax   : 3,   // 3\n" +
         "\n" +
         "    // Keyboard Settings\n" +
         "    keyboardSupport   : true,  // option\n" +
         "    arrowScroll       : 50,    // [px]\n" +
         "\n" +
         "    // Other\n" +
         "    fixedBackground   : false, \n" +
         "    excluded          : ''    \n" +
         "};\n" +
         "\n" +
         "var options = defaultOptions;\n" +
         "\n" +
         "\n" +
         "// Other Variables\n" +
         "var isExcluded = false;\n" +
         "var isFrame = false;\n" +
         "var direction = { x: 0, y: 0 };\n" +
         "var initDone  = false;\n" +
         "var root = document.documentElement;\n" +
         "var activeElement;\n" +
         "var observer;\n" +
         "var refreshSize;\n" +
         "var deltaBuffer = [];\n" +
         "var deltaBufferTimer;\n" +
         "var isMac = /^Mac/.test(navigator.platform);\n" +
         "\n" +
         "var key = { left: 37, up: 38, right: 39, down: 40, spacebar: 32, \n" +
         "            pageup: 33, pagedown: 34, end: 35, home: 36 };\n" +
         "var arrowKeys = { 37: 1, 38: 1, 39: 1, 40: 1 };\n" +
         "\n" +
         "/***********************************************\n" +
         " * INITIALIZE\n" +
         " ***********************************************/\n" +
         "\n" +
         "/**\n" +
         " * Tests if smooth scrolling is allowed. Shuts down everything if not.\n" +
         " */\n" +
         "function initTest() {\n" +
         "    if (options.keyboardSupport) {\n" +
         "        addEvent('keydown', keydown);\n" +
         "    }\n" +
         "}\n" +
         "\n" +
         "/**\n" +
         " * Sets up scrolls array, determines if frames are involved.\n" +
         " */\n" +
         "function init() {\n" +
         "  \n" +
         "    if (initDone || !document.body) return;\n" +
         "\n" +
         "    initDone = true;\n" +
         "\n" +
         "    var body = document.body;\n" +
         "    var html = document.documentElement;\n" +
         "    var windowHeight = window.innerHeight; \n" +
         "    var scrollHeight = body.scrollHeight;\n" +
         "    \n" +
         "    // check compat mode for root element\n" +
         "    root = (document.compatMode.indexOf('CSS') >= 0) ? html : body;\n" +
         "    activeElement = body;\n" +
         "    \n" +
         "    initTest();\n" +
         "\n" +
         "    // Checks if this script is running in a frame\n" +
         "    if (top != self) {\n" +
         "        isFrame = true;\n" +
         "    }\n" +
         "\n" +
         "    /**\n" +
         "     * Safari 10 fixed it, Chrome fixed it in v45:\n" +
         "     * This fixes a bug where the areas left and right to \n" +
         "     * the content does not trigger the onmousewheel event\n" +
         "     * on some pages. e.g.: html, body { height: 100% }\n" +
         "     */\n" +
         "    else if (isOldSafari &&\n" +
         "             scrollHeight > windowHeight &&\n" +
         "            (body.offsetHeight <= windowHeight || \n" +
         "             html.offsetHeight <= windowHeight)) {\n" +
         "\n" +
         "        var fullPageElem = document.createElement('div');\n" +
         "        fullPageElem.style.cssText = 'position:absolute; z-index:-10000; ' +\n" +
         "                                     'top:0; left:0; right:0; height:' + \n" +
         "                                      root.scrollHeight + 'px';\n" +
         "        document.body.appendChild(fullPageElem);\n" +
         "        \n" +
         "        // DOM changed (throttled) to fix height\n" +
         "        var pendingRefresh;\n" +
         "        refreshSize = function () {\n" +
         "            if (pendingRefresh) return; // could also be: clearTimeout(pendingRefresh);\n" +
         "            pendingRefresh = setTimeout(function () {\n" +
         "                if (isExcluded) return; // could be running after cleanup\n" +
         "                fullPageElem.style.height = '0';\n" +
         "                fullPageElem.style.height = root.scrollHeight + 'px';\n" +
         "                pendingRefresh = null;\n" +
         "            }, 500); // act rarely to stay fast\n" +
         "        };\n" +
         "  \n" +
         "        setTimeout(refreshSize, 10);\n" +
         "\n" +
         "        addEvent('resize', refreshSize);\n" +
         "\n" +
         "        // TODO: attributeFilter?\n" +
         "        var config = {\n" +
         "            attributes: true, \n" +
         "            childList: true, \n" +
         "            characterData: false \n" +
         "            // subtree: true\n" +
         "        };\n" +
         "\n" +
         "        observer = new MutationObserver(refreshSize);\n" +
         "        observer.observe(body, config);\n" +
         "\n" +
         "        if (root.offsetHeight <= windowHeight) {\n" +
         "            var clearfix = document.createElement('div');   \n" +
         "            clearfix.style.clear = 'both';\n" +
         "            body.appendChild(clearfix);\n" +
         "        }\n" +
         "    }\n" +
         "\n" +
         "    // disable fixed background\n" +
         "    if (!options.fixedBackground && !isExcluded) {\n" +
         "        body.style.backgroundAttachment = 'scroll';\n" +
         "        html.style.backgroundAttachment = 'scroll';\n" +
         "    }\n" +
         "}\n" +
         "\n" +
         "/**\n" +
         " * Removes event listeners and other traces left on the page.\n" +
         " */\n" +
         "function cleanup() {\n" +
         "    observer && observer.disconnect();\n" +
         "    removeEvent(wheelEvent, wheel);\n" +
         "    removeEvent('mousedown', mousedown);\n" +
         "    removeEvent('keydown', keydown);\n" +
         "    removeEvent('resize', refreshSize);\n" +
         "    removeEvent('load', init);\n" +
         "}\n" +
         "\n" +
         "\n" +
         "/************************************************\n" +
         " * SCROLLING \n" +
         " ************************************************/\n" +
         " \n" +
         "var que = [];\n" +
         "var pending = false;\n" +
         "var lastScroll = Date.now();\n" +
         "\n" +
         "/**\n" +
         " * Pushes scroll actions to the scrolling queue.\n" +
         " */\n" +
         "function scrollArray(elem, left, top) {\n" +
         "    \n" +
         "    directionCheck(left, top);\n" +
         "\n" +
         "    if (options.accelerationMax != 1) {\n" +
         "        var now = Date.now();\n" +
         "        var elapsed = now - lastScroll;\n" +
         "        if (elapsed < options.accelerationDelta) {\n" +
         "            var factor = (1 + (50 / elapsed)) / 2;\n" +
         "            if (factor > 1) {\n" +
         "                factor = Math.min(factor, options.accelerationMax);\n" +
         "                left *= factor;\n" +
         "                top  *= factor;\n" +
         "            }\n" +
         "        }\n" +
         "        lastScroll = Date.now();\n" +
         "    }          \n" +
         "    \n" +
         "    // push a scroll command\n" +
         "    que.push({\n" +
         "        x: left, \n" +
         "        y: top, \n" +
         "        lastX: (left < 0) ? 0.99 : -0.99,\n" +
         "        lastY: (top  < 0) ? 0.99 : -0.99, \n" +
         "        start: Date.now()\n" +
         "    });\n" +
         "        \n" +
         "    // don't act if there's a pending queue\n" +
         "    if (pending) {\n" +
         "        return;\n" +
         "    }  \n" +
         "\n" +
         "    var scrollRoot = getScrollRoot();\n" +
         "    var isWindowScroll = (elem === scrollRoot || elem === document.body);\n" +
         "    \n" +
         "    // if we haven't already fixed the behavior, \n" +
         "    // and it needs fixing for this sesh\n" +
         "    if (elem.$scrollBehavior == null && isScrollBehaviorSmooth(elem)) {\n" +
         "        elem.$scrollBehavior = elem.style.scrollBehavior;\n" +
         "        elem.style.scrollBehavior = 'auto';\n" +
         "    }\n" +
         "\n" +
         "    var step = function (time) {\n" +
         "        \n" +
         "        var now = Date.now();\n" +
         "        var scrollX = 0;\n" +
         "        var scrollY = 0; \n" +
         "    \n" +
         "        for (var i = 0; i < que.length; i++) {\n" +
         "            \n" +
         "            var item = que[i];\n" +
         "            var elapsed  = now - item.start;\n" +
         "            var finished = (elapsed >= options.animationTime);\n" +
         "            \n" +
         "            // scroll position: [0, 1]\n" +
         "            var position = (finished) ? 1 : elapsed / options.animationTime;\n" +
         "            \n" +
         "            // easing [optional]\n" +
         "            if (options.pulseAlgorithm) {\n" +
         "                position = pulse(position);\n" +
         "            }\n" +
         "            \n" +
         "            // only need the difference\n" +
         "            var x = (item.x * position - item.lastX) >> 0;\n" +
         "            var y = (item.y * position - item.lastY) >> 0;\n" +
         "            \n" +
         "            // add this to the total scrolling\n" +
         "            scrollX += x;\n" +
         "            scrollY += y;            \n" +
         "            \n" +
         "            // update last values\n" +
         "            item.lastX += x;\n" +
         "            item.lastY += y;\n" +
         "        \n" +
         "            // delete and step back if it's over\n" +
         "            if (finished) {\n" +
         "                que.splice(i, 1); i--;\n" +
         "            }           \n" +
         "        }\n" +
         "\n" +
         "        // scroll left and top\n" +
         "        if (isWindowScroll) {\n" +
         "            window.scrollBy(scrollX, scrollY);\n" +
         "        } \n" +
         "        else {\n" +
         "            if (scrollX) elem.scrollLeft += scrollX;\n" +
         "            if (scrollY) elem.scrollTop  += scrollY;                    \n" +
         "        }\n" +
         "        \n" +
         "        // clean up if there's nothing left to do\n" +
         "        if (!left && !top) {\n" +
         "            que = [];\n" +
         "        }\n" +
         "        \n" +
         "        if (que.length) { \n" +
         "            requestFrame(step, elem, (1000 / options.frameRate + 1)); \n" +
         "        } else { \n" +
         "            pending = false;\n" +
         "            // restore default behavior at the end of scrolling sesh\n" +
         "            if (elem.$scrollBehavior != null) {\n" +
         "                elem.style.scrollBehavior = elem.$scrollBehavior;\n" +
         "                elem.$scrollBehavior = null;\n" +
         "            }\n" +
         "        }\n" +
         "    };\n" +
         "    \n" +
         "    // start a new queue of actions\n" +
         "    requestFrame(step, elem, 0);\n" +
         "    pending = true;\n" +
         "}\n" +
         "\n" +
         "\n" +
         "/***********************************************\n" +
         " * EVENTS\n" +
         " ***********************************************/\n" +
         "\n" +
         "/**\n" +
         " * Mouse wheel handler.\n" +
         " * @param {Object} event\n" +
         " */\n" +
         "function wheel(event) {\n" +
         "\n" +
         "    if (!initDone) {\n" +
         "        init();\n" +
         "    }\n" +
         "    \n" +
         "    var target = event.target;\n" +
         "\n" +
         "    // leave early if default action is prevented   \n" +
         "    // or it's a zooming event with CTRL \n" +
         "    if (event.defaultPrevented || event.ctrlKey) {\n" +
         "        return true;\n" +
         "    }\n" +
         "    \n" +
         "    // leave embedded content alone (flash & pdf)\n" +
         "    if (isNodeName(activeElement, 'embed') || \n" +
         "       (isNodeName(target, 'embed') && /\\.pdf/i.test(target.src)) ||\n" +
         "        isNodeName(activeElement, 'object') ||\n" +
         "        target.shadowRoot) {\n" +
         "        return true;\n" +
         "    }\n" +
         "\n" +
         "    var deltaX = -event.wheelDeltaX || event.deltaX || 0;\n" +
         "    var deltaY = -event.wheelDeltaY || event.deltaY || 0;\n" +
         "    \n" +
         "    if (isMac) {\n" +
         "        if (event.wheelDeltaX && isDivisible(event.wheelDeltaX, 120)) {\n" +
         "            deltaX = -120 * (event.wheelDeltaX / Math.abs(event.wheelDeltaX));\n" +
         "        }\n" +
         "        if (event.wheelDeltaY && isDivisible(event.wheelDeltaY, 120)) {\n" +
         "            deltaY = -120 * (event.wheelDeltaY / Math.abs(event.wheelDeltaY));\n" +
         "        }\n" +
         "    }\n" +
         "    \n" +
         "    // use wheelDelta if deltaX/Y is not available\n" +
         "    if (!deltaX && !deltaY) {\n" +
         "        deltaY = -event.wheelDelta || 0;\n" +
         "    }\n" +
         "\n" +
         "    // line based scrolling (Firefox mostly)\n" +
         "    if (event.deltaMode === 1) {\n" +
         "        deltaX *= 40;\n" +
         "        deltaY *= 40;\n" +
         "    }\n" +
         "\n" +
         "    var overflowing = overflowingAncestor(target);\n" +
         "\n" +
         "    // nothing to do if there's no element that's scrollable\n" +
         "    if (!overflowing) {\n" +
         "        // except Chrome iframes seem to eat wheel events, which we need to \n" +
         "        // propagate up, if the iframe has nothing overflowing to scroll\n" +
         "        if (isFrame && isChrome)  {\n" +
         "            // change target to iframe element itself for the parent frame\n" +
         "            Object.defineProperty(event, \"target\", {value: window.frameElement});\n" +
         "            return parent.wheel(event);\n" +
         "        }\n" +
         "        return true;\n" +
         "    }\n" +
         "    \n" +
         "    // check if it's a touchpad scroll that should be ignored\n" +
         "    if (isTouchpad(deltaY)) {\n" +
         "        return true;\n" +
         "    }\n" +
         "\n" +
         "    // scale by step size\n" +
         "    // delta is 120 most of the time\n" +
         "    // synaptics seems to send 1 sometimes\n" +
         "    if (Math.abs(deltaX) > 1.2) {\n" +
         "        deltaX *= options.stepSize / 120;\n" +
         "    }\n" +
         "    if (Math.abs(deltaY) > 1.2) {\n" +
         "        deltaY *= options.stepSize / 120;\n" +
         "    }\n" +
         "    \n" +
         "    scrollArray(overflowing, deltaX, deltaY);\n" +
         "    event.preventDefault();\n" +
         "    scheduleClearCache();\n" +
         "}\n" +
         "\n" +
         "/**\n" +
         " * Keydown event handler.\n" +
         " * @param {Object} event\n" +
         " */\n" +
         "function keydown(event) {\n" +
         "\n" +
         "    var target   = event.target;\n" +
         "    var modifier = event.ctrlKey || event.altKey || event.metaKey || \n" +
         "                  (event.shiftKey && event.keyCode !== key.spacebar);\n" +
         "    \n" +
         "    // our own tracked active element could've been removed from the DOM\n" +
         "    if (!document.body.contains(activeElement)) {\n" +
         "        activeElement = document.activeElement;\n" +
         "    }\n" +
         "\n" +
         "    // do nothing if user is editing text\n" +
         "    // or using a modifier key (except shift)\n" +
         "    // or in a dropdown\n" +
         "    // or inside interactive elements\n" +
         "    var inputNodeNames = /^(textarea|select|embed|object)$/i;\n" +
         "    var buttonTypes = /^(button|submit|radio|checkbox|file|color|image)$/i;\n" +
         "    if ( event.defaultPrevented ||\n" +
         "         inputNodeNames.test(target.nodeName) ||\n" +
         "         isNodeName(target, 'input') && !buttonTypes.test(target.type) ||\n" +
         "         isNodeName(activeElement, 'video') ||\n" +
         "         isInsideYoutubeVideo(event) ||\n" +
         "         target.isContentEditable || \n" +
         "         modifier ) {\n" +
         "      return true;\n" +
         "    }\n" +
         "\n" +
         "    // [spacebar] should trigger button press, leave it alone\n" +
         "    if ((isNodeName(target, 'button') ||\n" +
         "         isNodeName(target, 'input') && buttonTypes.test(target.type)) &&\n" +
         "        event.keyCode === key.spacebar) {\n" +
         "      return true;\n" +
         "    }\n" +
         "\n" +
         "    // [arrwow keys] on radio buttons should be left alone\n" +
         "    if (isNodeName(target, 'input') && target.type == 'radio' &&\n" +
         "        arrowKeys[event.keyCode])  {\n" +
         "      return true;\n" +
         "    }\n" +
         "    \n" +
         "    var shift, x = 0, y = 0;\n" +
         "    var overflowing = overflowingAncestor(activeElement);\n" +
         "\n" +
         "    if (!overflowing) {\n" +
         "        // Chrome iframes seem to eat key events, which we need to \n" +
         "        // propagate up, if the iframe has nothing overflowing to scroll\n" +
         "        return (isFrame && isChrome) ? parent.keydown(event) : true;\n" +
         "    }\n" +
         "\n" +
         "    var clientHeight = overflowing.clientHeight; \n" +
         "\n" +
         "    if (overflowing == document.body) {\n" +
         "        clientHeight = window.innerHeight;\n" +
         "    }\n" +
         "\n" +
         "    switch (event.keyCode) {\n" +
         "        case key.up:\n" +
         "            y = -options.arrowScroll;\n" +
         "            break;\n" +
         "        case key.down:\n" +
         "            y = options.arrowScroll;\n" +
         "            break;         \n" +
         "        case key.spacebar: // (+ shift)\n" +
         "            shift = event.shiftKey ? 1 : -1;\n" +
         "            y = -shift * clientHeight * 0.9;\n" +
         "            break;\n" +
         "        case key.pageup:\n" +
         "            y = -clientHeight * 0.9;\n" +
         "            break;\n" +
         "        case key.pagedown:\n" +
         "            y = clientHeight * 0.9;\n" +
         "            break;\n" +
         "        case key.home:\n" +
         "            if (overflowing == document.body && document.scrollingElement)\n" +
         "                overflowing = document.scrollingElement;\n" +
         "            y = -overflowing.scrollTop;\n" +
         "            break;\n" +
         "        case key.end:\n" +
         "            var scroll = overflowing.scrollHeight - overflowing.scrollTop;\n" +
         "            var scrollRemaining = scroll - clientHeight;\n" +
         "            y = (scrollRemaining > 0) ? scrollRemaining + 10 : 0;\n" +
         "            break;\n" +
         "        case key.left:\n" +
         "            x = -options.arrowScroll;\n" +
         "            break;\n" +
         "        case key.right:\n" +
         "            x = options.arrowScroll;\n" +
         "            break;            \n" +
         "        default:\n" +
         "            return true; // a key we don't care about\n" +
         "    }\n" +
         "\n" +
         "    scrollArray(overflowing, x, y);\n" +
         "    event.preventDefault();\n" +
         "    scheduleClearCache();\n" +
         "}\n" +
         "\n" +
         "/**\n" +
         " * Mousedown event only for updating activeElement\n" +
         " */\n" +
         "function mousedown(event) {\n" +
         "    activeElement = event.target;\n" +
         "}\n" +
         "\n" +
         "\n" +
         "/***********************************************\n" +
         " * OVERFLOW\n" +
         " ***********************************************/\n" +
         "\n" +
         "var uniqueID = (function () {\n" +
         "    var i = 0;\n" +
         "    return function (el) {\n" +
         "        return el.uniqueID || (el.uniqueID = i++);\n" +
         "    };\n" +
         "})();\n" +
         "\n" +
         "var cacheX = {}; // cleared out after a scrolling session\n" +
         "var cacheY = {}; // cleared out after a scrolling session\n" +
         "var clearCacheTimer;\n" +
         "var smoothBehaviorForElement = {};\n" +
         "\n" +
         "//setInterval(function () { cache = {}; }, 10 * 1000);\n" +
         "\n" +
         "function scheduleClearCache() {\n" +
         "    clearTimeout(clearCacheTimer);\n" +
         "    clearCacheTimer = setInterval(function () { \n" +
         "        cacheX = cacheY = smoothBehaviorForElement = {}; \n" +
         "    }, 1*1000);\n" +
         "}\n" +
         "\n" +
         "function setCache(elems, overflowing, x) {\n" +
         "    var cache = x ? cacheX : cacheY;\n" +
         "    for (var i = elems.length; i--;)\n" +
         "        cache[uniqueID(elems[i])] = overflowing;\n" +
         "    return overflowing;\n" +
         "}\n" +
         "\n" +
         "function getCache(el, x) {\n" +
         "    return (x ? cacheX : cacheY)[uniqueID(el)];\n" +
         "}\n" +
         "\n" +
         "//  (body)                (root)\n" +
         "//         | hidden | visible | scroll |  auto  |\n" +
         "// hidden  |   no   |    no   |   YES  |   YES  |\n" +
         "// visible |   no   |   YES   |   YES  |   YES  |\n" +
         "// scroll  |   no   |   YES   |   YES  |   YES  |\n" +
         "// auto    |   no   |   YES   |   YES  |   YES  |\n" +
         "\n" +
         "function overflowingAncestor(el) {\n" +
         "    var elems = [];\n" +
         "    var body = document.body;\n" +
         "    var rootScrollHeight = root.scrollHeight;\n" +
         "    do {\n" +
         "        var cached = getCache(el, false);\n" +
         "        if (cached) {\n" +
         "            return setCache(elems, cached);\n" +
         "        }\n" +
         "        elems.push(el);\n" +
         "        if (rootScrollHeight === el.scrollHeight) {\n" +
         "            var topOverflowsNotHidden = overflowNotHidden(root) && overflowNotHidden(body);\n" +
         "            var isOverflowCSS = topOverflowsNotHidden || overflowAutoOrScroll(root);\n" +
         "            if (isFrame && isContentOverflowing(root) || \n" +
         "               !isFrame && isOverflowCSS) {\n" +
         "                return setCache(elems, getScrollRoot()); \n" +
         "            }\n" +
         "        } else if (isContentOverflowing(el) && overflowAutoOrScroll(el)) {\n" +
         "            return setCache(elems, el);\n" +
         "        }\n" +
         "    } while ((el = el.parentElement));\n" +
         "}\n" +
         "\n" +
         "function isContentOverflowing(el) {\n" +
         "    return (el.clientHeight + 10 < el.scrollHeight);\n" +
         "}\n" +
         "\n" +
         "// typically for <body> and <html>\n" +
         "function overflowNotHidden(el) {\n" +
         "    var overflow = getComputedStyle(el, '').getPropertyValue('overflow-y');\n" +
         "    return (overflow !== 'hidden');\n" +
         "}\n" +
         "\n" +
         "// for all other elements\n" +
         "function overflowAutoOrScroll(el) {\n" +
         "    var overflow = getComputedStyle(el, '').getPropertyValue('overflow-y');\n" +
         "    return (overflow === 'scroll' || overflow === 'auto');\n" +
         "}\n" +
         "\n" +
         "// for all other elements\n" +
         "function isScrollBehaviorSmooth(el) {\n" +
         "    var id = uniqueID(el);\n" +
         "    if (smoothBehaviorForElement[id] == null) {\n" +
         "        var scrollBehavior = getComputedStyle(el, '')['scroll-behavior'];\n" +
         "        smoothBehaviorForElement[id] = ('smooth' == scrollBehavior);\n" +
         "    }\n" +
         "    return smoothBehaviorForElement[id];\n" +
         "}\n" +
         "\n" +
         "\n" +
         "/***********************************************\n" +
         " * HELPERS\n" +
         " ***********************************************/\n" +
         "\n" +
         "function addEvent(type, fn, arg) {\n" +
         "    window.addEventListener(type, fn, arg || false);\n" +
         "}\n" +
         "\n" +
         "function removeEvent(type, fn, arg) {\n" +
         "    window.removeEventListener(type, fn, arg || false);  \n" +
         "}\n" +
         "\n" +
         "function isNodeName(el, tag) {\n" +
         "    return el && (el.nodeName||'').toLowerCase() === tag.toLowerCase();\n" +
         "}\n" +
         "\n" +
         "function directionCheck(x, y) {\n" +
         "    x = (x > 0) ? 1 : -1;\n" +
         "    y = (y > 0) ? 1 : -1;\n" +
         "    if (direction.x !== x || direction.y !== y) {\n" +
         "        direction.x = x;\n" +
         "        direction.y = y;\n" +
         "        que = [];\n" +
         "        lastScroll = 0;\n" +
         "    }\n" +
         "}\n" +
         "\n" +
         "if (window.localStorage && localStorage.SS_deltaBuffer) {\n" +
         "    try { // #46 Safari throws in private browsing for localStorage \n" +
         "        deltaBuffer = localStorage.SS_deltaBuffer.split(',');\n" +
         "    } catch (e) { } \n" +
         "}\n" +
         "\n" +
         "function isTouchpad(deltaY) {\n" +
         "    if (!deltaY) return;\n" +
         "    if (!deltaBuffer.length) {\n" +
         "        deltaBuffer = [deltaY, deltaY, deltaY];\n" +
         "    }\n" +
         "    deltaY = Math.abs(deltaY);\n" +
         "    deltaBuffer.push(deltaY);\n" +
         "    deltaBuffer.shift();\n" +
         "    clearTimeout(deltaBufferTimer);\n" +
         "    deltaBufferTimer = setTimeout(function () {\n" +
         "        try { // #46 Safari throws in private browsing for localStorage\n" +
         "            localStorage.SS_deltaBuffer = deltaBuffer.join(',');\n" +
         "        } catch (e) { }  \n" +
         "    }, 1000);\n" +
         "    var dpiScaledWheelDelta = deltaY > 120 && allDeltasDivisableBy(deltaY); // win64 \n" +
         "    var tp = !allDeltasDivisableBy(120) && !allDeltasDivisableBy(100) && !dpiScaledWheelDelta;\n" +
         "    if (deltaY < 50) return true;\n" +
         "    return tp;\n" +
         "} \n" +
         "\n" +
         "function isDivisible(n, divisor) {\n" +
         "    return (Math.floor(n / divisor) == n / divisor);\n" +
         "}\n" +
         "\n" +
         "function allDeltasDivisableBy(divisor) {\n" +
         "    return (isDivisible(deltaBuffer[0], divisor) &&\n" +
         "            isDivisible(deltaBuffer[1], divisor) &&\n" +
         "            isDivisible(deltaBuffer[2], divisor));\n" +
         "}\n" +
         "\n" +
         "function isInsideYoutubeVideo(event) {\n" +
         "    var elem = event.target;\n" +
         "    var isControl = false;\n" +
         "    if (document.URL.indexOf ('www.youtube.com/watch') != -1) {\n" +
         "        do {\n" +
         "            isControl = (elem.classList && \n" +
         "                         elem.classList.contains('html5-video-controls'));\n" +
         "            if (isControl) break;\n" +
         "        } while ((elem = elem.parentNode));\n" +
         "    }\n" +
         "    return isControl;\n" +
         "}\n" +
         "\n" +
         "var requestFrame = (function () {\n" +
         "      return (window.requestAnimationFrame       || \n" +
         "              window.webkitRequestAnimationFrame || \n" +
         "              window.mozRequestAnimationFrame    ||\n" +
         "              function (callback, element, delay) {\n" +
         "                 window.setTimeout(callback, delay || (1000/60));\n" +
         "             });\n" +
         "})();\n" +
         "\n" +
         "var MutationObserver = (window.MutationObserver || \n" +
         "                        window.WebKitMutationObserver ||\n" +
         "                        window.MozMutationObserver);  \n" +
         "\n" +
         "var getScrollRoot = (function() {\n" +
         "  var SCROLL_ROOT = document.scrollingElement;\n" +
         "  return function() {\n" +
         "    if (!SCROLL_ROOT) {\n" +
         "      var dummy = document.createElement('div');\n" +
         "      dummy.style.cssText = 'height:10000px;width:1px;';\n" +
         "      document.body.appendChild(dummy);\n" +
         "      var bodyScrollTop  = document.body.scrollTop;\n" +
         "      var docElScrollTop = document.documentElement.scrollTop;\n" +
         "      window.scrollBy(0, 3);\n" +
         "      if (document.body.scrollTop != bodyScrollTop)\n" +
         "        (SCROLL_ROOT = document.body);\n" +
         "      else \n" +
         "        (SCROLL_ROOT = document.documentElement);\n" +
         "      window.scrollBy(0, -3);\n" +
         "      document.body.removeChild(dummy);\n" +
         "    }\n" +
         "    return SCROLL_ROOT;\n" +
         "  };\n" +
         "})();\n" +
         "\n" +
         "\n" +
         "/***********************************************\n" +
         " * PULSE (by Michael Herf)\n" +
         " ***********************************************/\n" +
         " \n" +
         "/**\n" +
         " * Viscous fluid with a pulse for part and decay for the rest.\n" +
         " * - Applies a fixed force over an interval (a damped acceleration), and\n" +
         " * - Lets the exponential bleed away the velocity over a longer interval\n" +
         " * - Michael Herf, http://stereopsis.com/stopping/\n" +
         " */\n" +
         "function pulse_(x) {\n" +
         "    var val, start, expx;\n" +
         "    // test\n" +
         "    x = x * options.pulseScale;\n" +
         "    if (x < 1) { // acceleartion\n" +
         "        val = x - (1 - Math.exp(-x));\n" +
         "    } else {     // tail\n" +
         "        // the previous animation ended here:\n" +
         "        start = Math.exp(-1);\n" +
         "        // simple viscous drag\n" +
         "        x -= 1;\n" +
         "        expx = 1 - Math.exp(-x);\n" +
         "        val = start + (expx * (1 - start));\n" +
         "    }\n" +
         "    return val * options.pulseNormalize;\n" +
         "}\n" +
         "\n" +
         "function pulse(x) {\n" +
         "    if (x >= 1) return 1;\n" +
         "    if (x <= 0) return 0;\n" +
         "\n" +
         "    if (options.pulseNormalize == 1) {\n" +
         "        options.pulseNormalize /= pulse_(1);\n" +
         "    }\n" +
         "    return pulse_(x);\n" +
         "}\n" +
         "\n" +
         "\n" +
         "/***********************************************\n" +
         " * FIRST RUN\n" +
         " ***********************************************/\n" +
         "\n" +
         "var userAgent = window.navigator.userAgent;\n" +
         "var isEdge    = /Edge/.test(userAgent); // thank you MS\n" +
         "var isChrome  = /chrome/i.test(userAgent) && !isEdge; \n" +
         "var isSafari  = /safari/i.test(userAgent) && !isEdge; \n" +
         "var isMobile  = /mobile/i.test(userAgent);\n" +
         "var isIEWin7  = /Windows NT 6.1/i.test(userAgent) && /rv:11/i.test(userAgent);\n" +
         "var isOldSafari = isSafari && (/Version\\/8/i.test(userAgent) || /Version\\/9/i.test(userAgent));\n" +
         "var isEnabledForBrowser = (isChrome || isSafari || isIEWin7) && !isMobile;\n" +
         "\n" +
         "var supportsPassive = false;\n" +
         "try {\n" +
         "  window.addEventListener(\"test\", null, Object.defineProperty({}, 'passive', {\n" +
         "    get: function () {\n" +
         "            supportsPassive = true;\n" +
         "        } \n" +
         "    }));\n" +
         "} catch(e) {}\n" +
         "\n" +
         "var wheelOpt = supportsPassive ? { passive: false } : false;\n" +
         "var wheelEvent = 'onwheel' in document.createElement('div') ? 'wheel' : 'mousewheel'; \n" +
         "\n" +
         "if (wheelEvent && isEnabledForBrowser) {\n" +
         "    addEvent(wheelEvent, wheel, wheelOpt);\n" +
         "    addEvent('mousedown', mousedown);\n" +
         "    addEvent('load', init);\n" +
         "}\n" +
         "\n" +
         "\n" +
         "/***********************************************\n" +
         " * PUBLIC INTERFACE\n" +
         " ***********************************************/\n" +
         "\n" +
         "function SmoothScroll(optionsToSet) {\n" +
         "    for (var key in optionsToSet)\n" +
         "        if (defaultOptions.hasOwnProperty(key)) \n" +
         "            options[key] = optionsToSet[key];\n" +
         "}\n" +
         "SmoothScroll.destroy = cleanup;\n" +
         "\n" +
         "if (window.SmoothScrollOptions) // async API\n" +
         "    SmoothScroll(window.SmoothScrollOptions);\n" +
         "\n" +
         "if (typeof define === 'function' && define.amd)\n" +
         "    define(function() {\n" +
         "        return SmoothScroll;\n" +
         "    });\n" +
         "else if ('object' == typeof exports)\n" +
         "    module.exports = SmoothScroll;\n" +
         "else\n" +
         "    window.SmoothScroll = SmoothScroll;\n" +
         "\n" +
         "})();";
        public void updateController(TabController tabController)
        {
            if (tabController == null) return;
            this.tabController = tabController;
            LifeSpanHandler = new ExperimentalLifespanHandler(tabController);
            MenuHandler = new ContextMenuHandler(tabController);
            DisplayHandler = new DisplayHandler(tabController);
            LoadError += OnLoadError;
            Loaded += NetbirdChromium_Loaded;
        }

        private void NetbirdChromium_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        public void loadExtention()
        {

            var browser = WebBrowser;
            //The sample extension only works for http(s) schemes
            if (browser.Address.StartsWith("http"))
            {
                var requestContext = browser.GetBrowserHost().RequestContext;

                var dir = Path.Combine(AppContext.BaseDirectory, @"..\..\..\Extensions");
                dir = Path.GetFullPath(dir);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    throw new DirectoryNotFoundException("Unable to locate extensions folder - " + dir);
                }

                var extensionHandler = new ExtensionHandler
                {


                    LoadExtensionPopup = (url) =>
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            var extensionWindow = new Window();

                            var extensionBrowser = new ChromiumWebBrowser(url);
                            //extensionBrowser.IsBrowserInitializedChanged += (s, args) =>
                            //{
                            //    extensionBrowser.ShowDevTools();
                            //};

                            extensionWindow.Content = extensionBrowser;

                            extensionWindow.Show();
                        }));
                    },
                    GetActiveBrowser = (extension, isIncognito) =>
                    {
                        //Return the active browser for which the extension will act upon
                        return browser.BrowserCore;
                    }
                };



                requestContext.LoadExtensionsFromDirectory(dir, extensionHandler);

            }
            else
            {
                //MessageBox.Show("The sample extension only works with http(s) schemes, please load a different website and try again", "Unable to load Extension");
            }
        }

        public class ExtensionHandler : IExtensionHandler
        {
            public Func<IExtension, bool, IBrowser> GetActiveBrowser;
            public Action<string> LoadExtensionPopup;

            public void Dispose()
            {
                GetActiveBrowser = null;
                LoadExtensionPopup = null;
            }

            bool IExtensionHandler.CanAccessBrowser(IExtension extension, IBrowser browser, bool includeIncognito, IBrowser targetBrowser)
            {
                return false;
            }

            IBrowser IExtensionHandler.GetActiveBrowser(IExtension extension, IBrowser browser, bool includeIncognito)
            {
                return GetActiveBrowser?.Invoke(extension, includeIncognito);
                return null;
            }

            bool IExtensionHandler.GetExtensionResource(IExtension extension, IBrowser browser, string file, IGetExtensionResourceCallback callback)
            {
                return false;
            }

            bool IExtensionHandler.OnBeforeBackgroundBrowser(IExtension extension, string url, IBrowserSettings settings)
            {
                return false;
            }

            bool IExtensionHandler.OnBeforeBrowser(IExtension extension, IBrowser browser, IBrowser activeBrowser, int index, string url, bool active, IWindowInfo windowInfo, IBrowserSettings settings)
            {
                return false;
            }

            void IExtensionHandler.OnExtensionLoaded(IExtension extension)
            {
                var manifest = extension.Manifest;
                var browserAction = manifest["browser_action"].GetDictionary();
                if (browserAction.ContainsKey("default_popup"))
                {
                    var popupUrl = browserAction["default_popup"].GetString();

                    popupUrl = "chrome-extension://" + extension.Identifier + "/" + popupUrl;

                    LoadExtensionPopup?.Invoke(popupUrl);
                }
            }

            void IExtensionHandler.OnExtensionLoadFailed(CefErrorCode errorCode)
            {

            }

            void IExtensionHandler.OnExtensionUnloaded(IExtension extension)
            {

            }
        }

        public void OnLoadError(object chromiumWebBrowser, LoadErrorEventArgs loadErrorArgs)
        {
            String code = loadErrorArgs.ErrorCode.ToString();

            if (code == "Aborted") return;
            if (code == "BlockedByResponse") return;
            if (code == "EmptyResponse") return;
            if (code == "ConnectionReset") return;
            if (code == "QuicProtocolError") return;
            ((ChromiumWebBrowser) chromiumWebBrowser).GetMainFrame().LoadHtml("Netbird Error: " + loadErrorArgs.ErrorText + " #" + loadErrorArgs.ErrorCode);
            // throw new NotImplementedException();

        }

    }
}
