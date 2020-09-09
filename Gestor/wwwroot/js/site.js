$(document).ready(function () {
    $('span.form__error').css('opacity', '1');
    $('span.form__error').css('background-color', 'white');
    $('span.form__error').css('z-index', '100');
    $('#user_menu').click(VerificaNotificacoes);
    $('#icon-mobile').click(VerificaNotificacoes);
});

var verificandoNotificacoes = false;
function VerificaNotificacoes() {
    if (verificandoNotificacoes)
        return;

    verificandoNotificacoes = true;

    $.get('/Notificacoes/NovasNotificacoes')
        .done(function (result) {
            console.log(result);
            verificandoNotificacoes = false;

            if (result === true) {
                $('#novasMensagensMobile').show();
                $('#novasMensagens').show();
            } else {
                $('#novasMensagensMobile').hide();
                $('#novasMensagens').hide();
            }
        });
}

String.prototype.isEmpty = function () {
    return (this.length === 0 || !this.trim());
};

function DeletarRegistro(
    url,
    params,
    title = "Excluir Registro",
    message = "Tem certeza de que deseja excluir esse item? Essa alteração não poderá ser desfeita."
){
    $('#modal-excluir-title').html(title);
    $('#modal-excluir-message').html(message);
    $('#modal-excluir-ok').click(function () {
        $.post(url, params)
            .done(function () {
                $.fancybox.close();
                ShowMessage(
                    'Conteúdo Deletado!',
                    () => { window.location.reload(); }
                )
            })
            .fail(function () {
                $.fancybox.close();
                ShowMessage(
                    'Desculpe, mas tivemos algum problema.'
                )
            });
    });
    $('.modal-close').click(function () { $.fancybox.close(); });
    $.fancybox.open({
        src: '#modal-excluir'
    });
    return false;
};

function ShowMessage(message, ok = function () { $.fancybox.close(); }) {
    $('#modal-message-title').html(message);
    $.fancybox.open({
        src: '#modal-message',
        afterClose: ok
    });
}