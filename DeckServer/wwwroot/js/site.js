var connection = new signalR.HubConnectionBuilder().withUrl("/api/obsdock/v1/disconnect").build();
connection.start();
connection.onclose = connection.stop;

window.addEventListener('onbeforeunload', function (e) {
    e.preventDefault();
    connection.stop();
});

function updateWidgets(text)
{
    text = text.slice(0,-1);
    text.split("|").forEach((x) => {
        const parts = x.split(";", 2);
        try
        {
            document.getElementById(parts[0]).outerHTML = parts[1];
        }
        catch(err)
        {
            console.log(err);
        }
    }); 
}

function tick()
{
    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;
    function handleResponse()
    {
        if (xhr.readyState === XMLHttpRequest.DONE)
        {
            if (xhr.status === 200)
                updateWidgets(xhr.response);
            else
                console.error("Request failed with status ", xhr.status);
        }
    }
    
    function poll()
    {
        xhr.open("POST", "/api/obsdeck/v1/tick", true);
        xhr.onreadystatechange = handleResponse;
        xhr.send();
    }
    
    setInterval(poll, 1000);
}

console.log("Starting ticking the server for widget updates");
tick()