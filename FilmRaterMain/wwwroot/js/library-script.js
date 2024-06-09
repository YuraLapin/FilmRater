const Library = {
    data() {
        return {
            userName: localStorage.getItem("userName"),
            library: [],
            genreFilter: [],
            minScore: "",
            maxScore: "",
            minYear: "",
            maxYear: "",
            minYearDefault: "",
            maxYearDefault: "",
            genres: [],
        }
    },
    methods: {
        goHome() {
            $("#throw-to-login-form").submit()
        },
        async updateLibrary() {
            if (vm.minScore == "") {
                vm.minScore = 0
            }
            if (vm.maxScore == "") {
                vm.maxScore = 5
            }
            if (vm.minYear == "") {
                vm.minYear = vm.minYearDefault
            }
            if (vm.maxYear == "") {
                vm.maxYear = vm.maxYearDefault
            }
            await $.ajax({
                url: "api/requests/GetLibrary",
                method: "POST",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify({
                    "userName": vm.userName,
                    "genres": vm.genreFilter,
                    "minScore": vm.minScore,
                    "maxScore": vm.maxScore,
                    "minYear": vm.minYear,
                    "maxYear": vm.maxYear,
                }),
                success: function (result) {
                    vm.library = result
                    vm.library.forEach((film) => film.visualRating = film.currentUserRating)
                }
            })
        },
        async updateUserScore(filmId, userScore) {
            await $.ajax({
                url: "api/requests/UpdateUserScore",
                method: "POST",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify({
                    "filmId": filmId,
                    "userName": vm.userName,
                    "userScore": userScore,
                }),
            })
        },
        checkGenre(event, genre) {
            if (event.currentTarget.checked) {
                vm.genreFilter.push(genre)
            }
            else {
                vm.genreFilter.splice(vm.genreFilter.indexOf(genre), 1)
            }
        },
        tryEnterNumber(e) {
            if (!(Number(e.data) >= 0 && Number(e.data) <= 9)) {
                e.preventDefault();
            }
        }
    },    
}

const app = Vue.createApp(Library)

var vm = app.mount('#library')

window.onload = async function() {
    if (localStorage.getItem("userName") == '') {
        $("#throw-to-login-form").submit()
    }
    else {
        await $.ajax({
            url: "api/requests/GetMinMaxYears",
            method: "GET",
            dataType: "json",
            contentType: "application/json",
            success: function (result) {
                vm.minYear = result.minYear
                vm.minYearDefault = result.minYear
                vm.maxYear = result.maxYear
                vm.maxYearDefault = result.maxYear
            }
        })
        await $.ajax({
            url: "api/requests/GetGenres",
            method: "GET",
            dataType: "json",
            contentType: "application/json",
            success: function (result) {
                vm.genres = result
            }
        })
        vm.updateLibrary()
    }
}