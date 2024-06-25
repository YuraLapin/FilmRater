const FilmInfo = {
    data() {
        return {
            loggedIn: false,
            userName: "",
            filmId: passedFilmId,
            filmData: "",
            comments: [],
            loading: 0,
            showUserInfo: false,
            newComment: "",

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
        async getFullFilmData() {
            await $.ajax({
                url: "/api/requests/GetFullFilmData",
                method: "POST",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify({
                    "userName": vm.userName,
                    "filmId": vm.filmId,
                }),
                success: function (result) {
                    vm.filmData = result
                    vm.filmData.coverUrl = "/images/covers/" + passedFilmId + "_full.webp"
                    vm.filmData.visualRating = vm.filmData.currentUserRating
                }
            })
        },
        async getComments() {
            await $.ajax({
                url: "/api/requests/GetFilmComments",
                method: "POST",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify({
                    "filmId": vm.filmId,
                }),
                success: function (result) {
                    vm.comments = result.reverse()
                }
            })
        },
        async updateUserScore(userScore) {
            await $.ajax({
                url: "/api/requests/UpdateUserScore",
                method: "POST",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify({
                    "filmId": vm.filmId,
                    "userName": vm.userName,
                    "userScore": userScore,
                }),
                success: function (result) {
                    vm.filmData.currentUserRating = userScore
                },
            })
        },
        async sendComment() {
            await $.ajax({
                url: "/api/requests/UploadComment",
                method: "POST",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify({
                    "filmId": vm.filmId,
                    "userName": vm.userName,
                    "text": vm.newComment,
                }),
                success: function (result) {
                    vm.comments.unshift({
                        "userName": vm.userName,
                        "text": vm.newComment,
                        "userScore": vm.filmData.currentUserRating
                    })
                    vm.newComment = ""
                },
            })
        },
        async goToLibrary() {
            window.location.href = "/Home/Library"
        },
        logOut() {
            vm.userName = ""
            vm.loggedIn = false
            sessionStorage.removeItem("tokenKey")
            sessionStorage.removeItem("userName")
            window.location.reload()
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

const app = Vue.createApp(FilmInfo)

var vm = app.mount('#film-info')

window.onload = async function () {
    ++vm.loading

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

    vm.getFullFilmData()
    vm.getComments()

    --vm.loading
}
