
$(document).ready(function(){
    $("input").on("keyup",function(){  
      if($(this).val()) {
            $("button").removeAttr("disabled")
        } else {
            $("button").attr("disabled","disabled")
      }
    });

    $("button").click(function(){
        $.ajax({
            type: "POST",
            url: "http://localhost:5110/ShortLink",
            dataType: "jsonp",
            contentType : "application/json",
            crossDomain: true,
            data : JSON.stringify($("input").val()),
            processData: false,
        })
    })
});

