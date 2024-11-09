
// closes the multiselect when user presses escape
window.dropdownInterop = {
    registerEscapeKeyHandler: function (dotNetObject) {
        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape') {
                console.log('escape pressed');
                dotNetObject.invokeMethodAsync('CloseDropdown');
            }
        });
    }
};

window.saveAsFile = function(filename, fileContent)
{
    var link = document.createElement('a')
    link.download = filename
    // mime-types: https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types
    link.href = 'data:text/plain;charset=utf-8,'+encodeURIComponent(fileContent)
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
}