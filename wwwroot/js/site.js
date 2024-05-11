// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function bs()
{
    console.log('here bitches!!!')
}

const showHidePasswordField = (fieldId) => {

    $(`#${fieldId} a`).on('click', function(event) {

        event.preventDefault();

        if($(`#${fieldId} input`).attr("type") === "text"){

            $(`#${fieldId} input`).attr('type', 'password');

            $(`#${fieldId} i`).addClass( "fa-eye-slash" );

            $(`#${fieldId} i`).removeClass( "fa-eye" );

        }else if($(`#${fieldId} input`).attr("type") === "password"){

            $(`#${fieldId} input`).attr('type', 'text');

            $(`#${fieldId} i`).removeClass( "fa-eye-slash" );

            $(`#${fieldId} i`).addClass( "fa-eye" );

        }

    });

}