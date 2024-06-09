const AuthenticationForm = {
    data() {
        return {
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
        }
    },
    methods: {
        async tryLogIn() {
            vm.logInError = false
            vm.logInErrorFill = false
            vm.regErrorPassword = false
            vm.regErrorTaken = false
            vm.regErrorFill = false
            if (vm.login == "" || vm.password == "") {
                vm.logInErrorFill = true
            }
            else {
                await $.ajax({
                    url: "api/requests/TryLogIn",
                    method: "POST",
                    dataType: "json",
                    contentType: "application/json",
                    data: JSON.stringify({
                        "login": vm.login,
                        "password": vm.password,
                    }),
                    success: function (result) {
                        if (result == true) {
                            localStorage.setItem("userName", vm.login)
                            $("#open-library-form").submit()
                        }
                        else {
                            vm.logInError = true
                        }
                    }
                })
            }
        },
        async tryRegister() {
            vm.logInError = false
            vm.logInErrorFill = false
            vm.regErrorPassword = false
            vm.regErrorTaken = false
            vm.regErrorFill = false
            if (vm.password1 != vm.password2) {
                vm.regErrorPassword = true
            }
            else if (vm.loginRegister == "" || vm.password1 == "" || vm.password2 == "") {
                vm.regErrorFill = true
            }
            else {
                await $.ajax({
                    url: "api/requests/TryRegister",
                    method: "POST",
                    dataType: "json",
                    contentType: "application/json",
                    data: JSON.stringify({
                        "login": vm.loginRegister,
                        "password": vm.password1,
                    }),
                    success: function (result) {
                        if (result == true) {
                            localStorage.setItem("userName", vm.loginRegister)
                            $("#open-library-form").submit()
                        }
                        else {
                            vm.regErrorTaken = true
                        }
                    }
                })
            }
        }
    }
}

const app = Vue.createApp(AuthenticationForm)

var vm = app.mount('#authentication-form')