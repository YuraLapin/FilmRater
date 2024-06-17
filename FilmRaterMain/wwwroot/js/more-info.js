const FilmInfo = {
    data() {
        return {
            userName: passedUserName,
            filmId: passedFilmId,
            filmData: "",
            comments: [],
            loading: 0,
            showUserInfo: false,
            newComment: "",
        }
    },
    methods: {
        async getFullFilmData() {
            await $.ajax({
                url: "api/requests/GetFullFilmData",
                method: "POST",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify({
                    "userName": vm.userName,
                    "filmId": vm.filmId,
                }),
                success: function (result) {
                    vm.filmData = result
                    vm.filmData.coverUrl = "../images/covers/" + passedFilmId + "_full.webp"
                    vm.filmData.visualRating = vm.filmData.currentUserRating
                }
            })
        },
        async getComments() {
            await $.ajax({
                url: "api/requests/GetFilmComments",
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
                url: "api/requests/UpdateUserScore",
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
                url: "api/requests/UploadComment",
                method: "POST",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify({
                    "filmId": vm.filmId,
                    "userName": vm.userName,
                    "text": newComment,
                }),
                success: function (result) {
                    vm.comments.unshift({
                        "userName": vm.userName,
                        "text": newComment,
                        "userScore": vm.filmData.currentUserRating
                    })
                    newComment = ""
                },
            })
        },
        async goToLibrary() {
            $("#input-library-username").val(vm.userName)
            $("#go-library-form").submit()
        },
        async logOut() {
            vm.userName = ""
            vm.getFullFilmData()
        },
        async goLogin() {
            $("#input-login-camefrom").val("MoreInfo")
            $("#input-login-filmid").val(vm.filmId)
            $("#go-login-form").submit()
        },
    },
}

const app = Vue.createApp(FilmInfo)

var vm = app.mount('#film-info')

window.onload = function () {
    ++vm.loading
    vm.getFullFilmData()
    vm.getComments()
    --vm.loading
}
