const Library = {
    data() {
        return {
            userName: passedUserName,
            library: [],
            genreFilter: [],
            minScore: "",
            maxScore: "",
            minYear: "",
            maxYear: "",
            minYearDefault: "",
            maxYearDefault: "",
            genres: [],
            currentPage: 1,
            totalPages: "",
            loading: 0,
            showUserInfo: false,
        }
    },
    methods: {
        logOut() {
            vm.userName = ""
            vm.updateLibrary(vm.currentPage)
        },
        goLogin() {
            $("#input-login-camefrom").val("Library")
            $("#input-login-filmid").val(0)
            $("#throw-to-login-form").submit()
        },
        goToMoreFilmInfo(filmId) {
            $("#hidden-form-input-user-name").val(vm.userName)
            $("#hidden-form-input-film-id").val(filmId)
            $("#throw-to-more-info").submit()
        },
        async updateLibrary(page) {
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
            vm.library = []
            ++vm.loading
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
                    "page": page,
                }),
                success: function (result) {
                    vm.library = result
                    vm.library.forEach((film) => {
                        film.visualRating = film.currentUserRating
                        film.coverUrl = "../images/covers/" + film.id + "_full.webp"
                    })
                    vm.currentPage = page
                }
            }).always(function () {
                --vm.loading
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
        },
        async getTotalPages() {
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
                url: "api/requests/GetTotalPages",
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
                    "page": 1,
                }),
                success: function (result) {
                    vm.totalPages = result
                }
            })
        },
        createRange(start, stop, step) {
            return Array.from({ length: (stop - start) / step + 1 }, (_, i) => start + i * step)
        },
    },    
}

const app = Vue.createApp(Library)

var vm = app.mount('#library')

window.onload = async function () {
    ++vm.loading
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
    await vm.updateLibrary(1)
    await vm.getTotalPages()
    --vm.loading
}
