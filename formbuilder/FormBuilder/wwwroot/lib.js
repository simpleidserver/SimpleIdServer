var FormBuilder = FormBuilder || {};

FormBuilder.navigate = function (element) {
    element.click();
};

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