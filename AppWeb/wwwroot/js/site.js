function TagRecomendationInit(tagAddId, TagsRecomsId) {
    tagAdd = document.getElementById(tagAddId)
    tagAdd.oninput = GetTags
    tagAdd.onfocus = GetTags
    tagAdd.onblur = function () {
        TagsRecomHide()
    }
    TagsRecoms = document.getElementById(TagsRecomsId)
}

function GetTags() {
    if (this.value.length >= 3) {
        fetch("/Tag/GetAll?" + (new URLSearchParams({
            search: this.value
        })).toString(), {
            method: "GET"
        }).then((response) => {
            return response.json();
        }).then((data) => {
            if (data.length > 0) {
                let cord = getCaretGlobalCoordinates(this)
                TagsRecomSetCord(cord)
                TagsRecomShow(data)
            } else {
                TagsRecomHide()
            }
        })
    }
}

function TagsRecomHide() {
    TagsRecoms.innerHTML = ''
    TagsRecoms.style.display = 'none'
}

function TagsRecomSetCord(cord) {
    TagsRecoms.style.top = cord.top
    TagsRecoms.style.left = cord.left
}

function TagsRecomShow(data) {
    let str = ''
    data.forEach(v => str += `<div class="TagsRecom" onMouseDown="TagsRecomClick(this)">${v.Name}</div>`)
    TagsRecoms.innerHTML = str
    TagsRecoms.style.display = 'inline-block'
}

function TagsRecomClick(el) {
    AddTag(el.innerHTML)

    tagAdd.value = ''
    TagsRecomHide()
}

function getElementCoords(elem) {
    let box = elem.getBoundingClientRect();
    let body = document.body;
    let docEl = document.documentElement;
    let scrollTop = window.pageYOffset || docEl.scrollTop || body.scrollTop;
    let scrollLeft = window.pageXOffset || docEl.scrollLeft || body.scrollLeft;
    let clientTop = docEl.clientTop || body.clientTop || 0;
    let clientLeft = docEl.clientLeft || body.clientLeft || 0;
    let top = box.top + scrollTop - clientTop;
    let left = box.left + scrollLeft - clientLeft;
    return {top: Math.round(top), left: Math.round(left)};
}

function getCaretGlobalCoordinates(Desired_ID) {
    let coordinates = getCaretCoordinates(Desired_ID, Desired_ID.selectionStart);
    let elementCoordinates = getElementCoords(Desired_ID);
    let top = coordinates.top + elementCoordinates.top;
    let left = coordinates.left + elementCoordinates.left;
    return {top: `${top}px`, left: `${left}px`};
}

(function () {
    let properties = ['direction', 'boxSizing', 'width', 'height', 'overflowX', 'overflowY', 'borderTopWidth', 'borderRightWidth', 'borderBottomWidth', 'borderLeftWidth', 'borderStyle', 'paddingTop', 'paddingRight', 'paddingBottom', 'paddingLeft', 'fontStyle', 'fontVariant', 'fontWeight', 'fontStretch', 'fontSize', 'fontSizeAdjust', 'lineHeight', 'fontFamily', 'textAlign', 'textTransform', 'textIndent', 'textDecoration', 'letterSpacing', 'wordSpacing', 'tabSize', 'MozTabSize'];
    let isBrowser = (typeof window !== 'undefined');
    let isFirefox = (isBrowser && window.mozInnerScreenX != null);

    function getCaretCoordinates(element, position, options) {
        if (!isBrowser) {
            throw new Error('textarea-caret-position#getCaretCoordinates should only be called in a browser');
        }
        let debug = options && options.debug || false;
        if (debug) {
            let el = document.querySelector('#input-textarea-caret-position-mirror-div');
            if (el) {
                el.parentNode.removeChild(el);
            }
        }
        let div = document.createElement('div');
        div.id = 'input-textarea-caret-position-mirror-div';
        document.body.appendChild(div);
        let style = div.style;
        let computed = window.getComputedStyle ? getComputedStyle(element) : element.currentStyle;
        style.whiteSpace = 'pre-wrap';
        if (element.nodeName !== 'INPUT') style.wordWrap = 'break-word';
        style.position = 'absolute';
        if (!debug) style.visibility = 'hidden';
        properties.forEach(function (prop) {
            style[prop] = computed[prop];
        });
        if (isFirefox) {
            if (element.scrollHeight > parseInt(computed.height)) style.overflowY = 'scroll';
        } else {
            style.overflow = 'hidden';
        }
        div.textContent = element.value.substring(0, position);
        if (element.nodeName === 'INPUT') div.textContent = div.textContent.replace(/\s/g, '\u00a0');
        let span = document.createElement('span');
        span.textContent = element.value.substring(position) || '.';
        div.appendChild(span);
        let coordinates = {
            top: span.offsetTop + parseInt(computed['borderTopWidth']),
            left: span.offsetLeft + parseInt(computed['borderLeftWidth'])
        };
        if (debug) {
            span.style.backgroundColor = '#aaa';
        } else {
            document.body.removeChild(div);
        }
        return coordinates;
    }

    if (typeof module != 'undefined' && typeof module.exports != 'undefined') {
        module.exports = getCaretCoordinates;
    } else if (isBrowser) {
        window.getCaretCoordinates = getCaretCoordinates;
    }
}());

function MainPageLoadTopTags(id) {
    fetch("/Tag/GetTopTags", {
        method: "GET"
    }).then((response) => {
        return response.json();
    }).then((data) => {
        if (data.length > 0) {
            let ulTag = document.getElementById(id)
            let minFontSize = 1
            let maxFontSize = 2
            let liTags = ""
            let weights = []
            for (let v of data) {
                if (!weights.includes(v.Amount)) {
                    weights.push(v.Amount)
                }
            }
            weights.sort((a, b) => a - b)
            let shuffledArray = data.sort((a, b) => 0.5 - Math.random());
            for (let v of shuffledArray) {
                let title = v.Name
                let url = `/Tag/Get/${v.Id}`
                let m = weights.indexOf(v.Amount) + 1
                let a = ((m - weights[0]) / (weights.length - weights[0]))
                let dif = (maxFontSize - minFontSize)
                let font_size = a * dif + minFontSize
                liTags = liTags.concat(`<li>
              	<a href="${url}" style="font-size: ${font_size}em;">
                  	${title} [${v.Amount}]
                  </a>
              </li>`)
            }
            ulTag.innerHTML = liTags
        }
    })
}

let loading = 0

function LoadingGif(val) {
    let loadingDiv = document.getElementsByClassName('loading')[0]
    let body = document.getElementsByTagName('body')[0]
    loading += val
    if (loading > 0) {
        body.style.overflow = "hidden"
        loadingDiv.style.display = "block"
    } else {
        loadingDiv.style.display = "none"
        body.style.overflow = "visible"
    }
}

function GetReview(page, id) {
    LoadingGif(+1)
    fetch(page, {
        method: "GET"
    }).then((response) => {
        return response.text();
    }).then((data) => {
        document.getElementById(id).innerHTML = data
        LoadingGif(-1)
    })
}

function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}

function setCookie(name, value, days = 7) {
    let expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function CheckUiThemeCookie() {
    let value = getCookie("Theme")
    if (!value) {
        setCookie("Theme", "bootstrap_Sandstone.css")
    }
}

CheckUiThemeCookie()

function ChangeUiTheme() {
    let value = getCookie("Theme")
    if (value) {
        if (value === "bootstrap_Sandstone.css") {
            setCookie("Theme", "bootstrap_Slate.css")
        } else if (value === "bootstrap_Slate.css") {
            setCookie("Theme", "bootstrap_Sandstone.css")
        }
    }
    window.location.reload()
}