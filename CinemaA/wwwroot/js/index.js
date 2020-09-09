// FilmsSessions View

// button click event listener for filtering sessions by date
document.getElementById("FilterButton").addEventListener("click", function () {
    FilterByDate(document.getElementById("FilterDate").value);
}); 

// ajax request for filtering sessions
async function FilterByDate(FilterDate) { 
    let xsrf_token = document.getElementsByName("__RequestVerificationToken")[0].value;
    const url = '/FilmsSessions/FilterByDateAjax';

    try {
        const response = await fetch(url, {
            method: 'POST',
            body: JSON.stringify(FilterDate),
            credentials: 'include',
            headers: {
                "XSRF-TOKEN": xsrf_token,
                "Content-Type" : "application/json"
            }
        });
        if (response.status === 200) {
            const json = await response.json();
            ShowFilteredFilmsSessions(FilterDate, json.films);
        }
        else {
            const text = await response.text();
            alert(text);
        }
    } catch (error) {
        console.error('Ошибка:', error);
    }
}

// show found films sessions on page
function ShowFilteredFilmsSessions(date, films) {
    document.getElementById("FilterDate").value = date;
    var html = "";
    if (films.value.length == 0) {
        html += `<p style="margin: 10px;">There are no sessions for this date. Sorry!</p>`;
    }
    else {
        for (let i = 0; i < films.value.length; i++) {
            html += `<div class="film">
            <p>Title: ${films.value[i].title}</p>
            <p>Annotation: ${films.value[i].annotation}</p>
            <p>Genre: ${films.value[i].genre}</p>
            <p>Made In: ${films.value[i].madeIn}</p>
            <p>Format: ${films.value[i].format}</p>
            <div>`;
            for (let j = 0; j < films.value[i].session.length; j++) {
                html += `<a class="btn btn-info" style="margin: 5px;" href="/SessionTickets?IdSession=${films.value[i].session[j].id}">
                            ${films.value[i].session[j].time.slice(0, -3)}
                         </a>`;
            }
            html += `</div></div>`;
        }
    }
    document.getElementById("filteredSessions").innerHTML = html;
}