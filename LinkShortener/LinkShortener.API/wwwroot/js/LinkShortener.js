$(document).ready(function () {
    $("input").on("keyup", function () {
        if ($(this).val()) {
            $(".btn").removeAttr("disabled")
        } else {
            $(".btn").attr("disabled", "disabled")
        }
    });

    $("button").click(function () {
        $.ajax({
            type: "POST",
            url: "http://localhost:5110/ShortLink",
            contentType: "application/json",
            crossDomain: true,
            data: JSON.stringify($("input").val()),
            processData: false,
            success: function (data) {
                const alertPlaceholder = $('#alertPlaceholder');
                alertPlaceholder.empty();
                const appendAlert = (message) => {
                    console.log("qwe");
                    const alertHtml = `
                    <div class="linkAlert" role="alert">
                        <a href=${message} class='linkAlert'>${message}</a>
                    </div>
                    `;
                    alertPlaceholder.append(alertHtml);
                };
                console.log(data, "primary");
                appendAlert(data, "primary")
            },
            error: function (xhr, status, error) {
                console.log('Error:', error);
                console.log('Status:', status);
                console.log('Response:', xhr.responseText);
            }
        })
    })
});

