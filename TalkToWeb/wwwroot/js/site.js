function TesteCors() {
    var tokenJWT = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Ik1hdXJvQGVtYWlsLmNvbSIsInN1YiI6ImFhM2QxOWM0LTc5YzMtNDk3Ny1hN2NlLWVlMmRkNjAwODIyZSIsIm5iZiI6MTU5NTI3ODQ1NCwiZXhwIjoxNTk1MjgyMDU0LCJpYXQiOjE1OTUyNzg0NTR9.NEBpxEyaIz-SGnVYBymuqmqLCBabpnJVEaMY8NMzIq4";
    var servico = " https://localhost:44372/api/mensagem/cdef900c-c351-48c1-9017-b2e9691b4890/c008e8df-ddf3-4f95-b0db-6c14cee0a00d";
    $("#resultado").html("----Solicitando----");
    $.ajax({
        url: servico,
        method: "GET",
        crossDomain: true,
        headers: { "Accept": "application/json"},
        beforeSend: function (xhr) {  xhr.setRequestHeader("Authorization", "Bearer" + tokenJWT);},
        success: function (data, status, xhr) {
            $("#resultado").html(data);
            console.info(data);
        }
        });
}