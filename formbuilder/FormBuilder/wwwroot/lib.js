var FormBuilder = FormBuilder || {};

FormBuilder.navigate = function (element) {
    element.click();
};

FormBuilder.getSize = function (id) {
    var element = document.getElementById(id);
    return { height: element.offsetHeight, width: element.offsetWidth };
}

FormBuilder.getPosition = function (elt) {
    const rect = elt.getBoundingClientRect();
    return { left: rect.left, top: rect.top };
}