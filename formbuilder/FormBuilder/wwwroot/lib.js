var FormBuilder = FormBuilder || {};
FormBuilder.getSize = function (elt) {
    return { height: elt.offsetHeight, width: elt.offsetWidth };
}

FormBuilder.getPosition = function (elt) {
    const rect = elt.getBoundingClientRect();
    return { X: rect.left, Y: rect.top };
}

FormBuilder.getOffsetPosition = function (elt) {
    return { X: elt.offsetLeft, Y: elt.offsetTop };
}

FormBuilder.getPointInSvgSpace = function (clientX, clientY, svg) {
    let pt = new DOMPoint(clientX, clientY);
    pt = pt.matrixTransform(svg.getScreenCTM().inverse());
    return { X: pt.x, Y: pt.y };
}

FormBuilder.navigate = function (url) {
    window.open(url, '_blank').focus();
}

FormBuilder.navigateForce = function (url) {
    window.location.href = url;
}

FormBuilder.submitForm = function (url, json, method) {
    const div = document.createElement("div");
    var form = "<form id='tmpForm' action='"+ url +"' method='"+ method +"'>";
    for (var record in json) {
        var value = json[record] ?? "";
        form += "<input type='hidden' name='" + record + "' value='" + value +"' />";
    }

    form += "</form>";
    div.innerHTML = form;
    document.body.append(div);
    const tmpForm = document.getElementById("tmpForm");
    tmpForm.submit();
}

FormBuilder.refreshCss = function (id, cssContent) {
    var styleElement = document.getElementById(id);
    if (!styleElement) {
        return;
    }

    styleElement.removeAttribute('href');
    styleElement.id = id;
    styleElement.innerHTML = cssContent;
};