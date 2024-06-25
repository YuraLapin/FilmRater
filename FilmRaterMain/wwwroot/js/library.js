const Library = {
    data() {
        return {
            loggedIn: false,
            userName: "",
            library: [],
            genreFilter: [],
            minScore: "",
            maxScore: "",
            minYear: "",
            maxYear: "",
            nameFilter: "",
            minYearDefault: "",
            maxYearDefault: "",
            genres: [],
            currentPage: 1,
            totalPages: "",
            loading: 1,
            showUserInfo: false,

            login: "",
            password: "",
            loginRegister: "",
            password1: "",
            password2: "",
            regMode: false,
            logInError: false,
            logInErrorFill: false,
            regErrorPassword: false,
            regErrorTaken: false,
            regErrorFill: false,
            regErrorLoginTooLong: false,
            regErrorPasswordTooLong: false,
            regErrorLoginTooShort: false,
            regErrorPasswordTooShort: false,
            regErrorLoginSymbols: false,
            regErrorPasswordSymbols: false,
        }
    },
    methods: {
        logOut() {
            vm.userName = ""
            vm.loggedIn = false
            sessionStorage.removeItem("tokenKey")
            sessionStorage.removeItem("userName")
            window.location.reload()
        },
        goMoreInfo(filmId) {
            window.location.href = `/Home/Library/${filmId}`
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
                url: "/api/requests/GetLibrary",
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
                    "nameFilter": vm.nameFilter,
                }),
                success: function (result) {
                    vm.library = result
                    vm.library.forEach((film) => {
                        film.visualRating = film.currentUserRating
                        film.coverUrl = "/images/covers/" + film.id + "_full.webp"
                    })
                    vm.currentPage = page
                }
            }).always(function () {
                --vm.loading
            })
        },
        async updateUserScore(filmId, userScore) {
            await $.ajax({
                url: "/api/requests/UpdateUserScore",
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
                url: "/api/requests/GetTotalPages",
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
                    "nameFilter": vm.nameFilter,
                }),
                success: function (result) {
                    vm.totalPages = result
                }
            })
        },
        createRange(start, stop, step) {
            return Array.from({ length: (stop - start) / step + 1 }, (_, i) => start + i * step)
        },
        checkKeyCodeAndUpdate(e) {
            if (e.keyCode == 13) {
                vm.updateLibrary(1)
            }
        },
        async tryLogIn() {
            vm.logInError = false
            vm.logInErrorFill = false

            vm.regErrorPassword = false
            vm.regErrorTaken = false
            vm.regErrorFill = false
            vm.regErrorLoginTooLong = false
            vm.regErrorPasswordTooLong = false
            vm.regErrorLoginTooShort = false
            vm.regErrorPasswordTooShort = false
            vm.regErrorLoginSymbols = false
            vm.regErrorPasswordSymbols = false

            if (vm.login == "" || vm.password == "") {
                vm.logInErrorFill = true
                return
            }

            await $.ajax({
                url: "/Home/TryLogIn",
                method: "POST",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify({
                    "userName": vm.login,
                    "password": vm.password,
                }),
                success: function (response) {
                    if (response == false) {
                        vm.logInError = true
                    }
                    else {
                        sessionStorage.setItem("tokenKey", response.access_token)
                        sessionStorage.setItem("userName", response.username)
                        window.location.reload()
                    }
                },
            })
        },
        async tryRegister() {
            vm.logInError = false
            vm.logInErrorFill = false

            vm.regErrorPassword = false
            vm.regErrorTaken = false
            vm.regErrorFill = false
            vm.regErrorLoginTooLong = false
            vm.regErrorPasswordTooLong = false
            vm.regErrorLoginTooShort = false
            vm.regErrorPasswordTooShort = false
            vm.regErrorLoginSymbols = false
            vm.regErrorPasswordSymbols = false

            if (vm.loginRegister == "" || vm.password1 == "" || vm.password2 == "") {
                vm.regErrorFill = true
                return
            }

            if (vm.password1 != vm.password2) {
                vm.regErrorPassword = true
                return
            }

            if (vm.loginRegister.length > 12) {
                vm.regErrorLoginTooLong = true
                return
            }

            if (vm.loginRegister.length < 5) {
                vm.regErrorLoginTooShort = true
                return
            }

            if (vm.password1.length > 12) {
                vm.regErrorPasswordTooLong = true
                return
            }

            if (vm.password1.length < 5) {
                vm.regErrorPasswordTooShort = true
                return
            }

            const allowedSymbols = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-_"

            for (const c of Array.from(vm.loginRegister)) {
                if (!allowedSymbols.includes(c)) {
                    vm.regErrorLoginSymbols = true
                    return
                }
            }

            for (const c of Array.from(vm.password1)) {
                if (!allowedSymbols.includes(c)) {
                    vm.regErrorPasswordSymbols = true
                    return
                }
            }

            await $.ajax({
                url: "/Home/TryRegister",
                method: "POST",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify({
                    userName: vm.loginRegister,
                    password: vm.password1,
                }),
                success: function (response) {
                    if (response == false) {
                        vm.regErrorPassword = true
                    }
                    else {
                        sessionStorage.setItem("tokenKey", response.access_token)
                        sessionStorage.setItem("userName", response.username)
                        window.location.reload()
                    }
                },
            })
        },
    },    
}

const app = Vue.createApp(Library)

var vm = app.mount('#library')

window.onload = async function () {
    const token = sessionStorage.getItem("tokenKey")
    await $.ajax({
        url: "/Home/CheckToken",
        method: "GET",
        headers: {
            "Accept": "application/json",
            "Authorization": "Bearer " + token,
        },
        success: function (response) {
            if (response == true) {
                vm.userName = sessionStorage.getItem("userName")
                vm.loggedIn = true
            }
        },
    })

    await $.ajax({
        url: "/api/requests/GetMinMaxYears",
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
        url: "/api/requests/GetGenres",
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
