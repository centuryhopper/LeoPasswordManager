function saveAsFile(filename, fileContent)
{
    var link = document.createElement('a')
    link.download = filename
    // mime-types: https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types
    link.href = 'data:text/plain;charset=utf-8,'+encodeURIComponent(fileContent)
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
}

function showHidePasswordField(fieldId)
{
    $(`#${fieldId} a`).on('click', function(event) {
        event.preventDefault();
        console.log(fieldId)
        if($(`#${fieldId} input`).attr("type") === "text")
        {
            $(`#${fieldId} input`).attr('type', 'password');
            $(`#${fieldId} i`).addClass( "fa-eye-slash" );
            $(`#${fieldId} i`).removeClass( "fa-eye" );
        }
        else if($(`#${fieldId} input`).attr("type") === "password")
        {
            $(`#${fieldId} input`).attr('type', 'text');
            $(`#${fieldId} i`).removeClass( "fa-eye-slash" );
            $(`#${fieldId} i`).addClass( "fa-eye" );
        }
    });

}