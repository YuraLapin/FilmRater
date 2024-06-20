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
            regErrorLoginTooLong: false,
            regErrorPasswordTooLong: false,
            regErrorLoginTooShort: false,
            regErrorPasswordTooShort: false,
            regErrorLoginSymbols: false,
            regErrorPasswordSymbols: false,
        }
    },
    methods: {
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
                url: "TryLogIn",
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
                        $("#go-library-form").submit()
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

            for (const c of Array.from(vm.loginRegister))  {
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
                url: "TryRegister",
                method: "POST",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify({
                    "userName": vm.login,
                    "password": vm.password,
                }),
                success: function (response) {
                    if (response == false) {
                        vm.regErrorPassword = true
                    }
                    else {
                        sessionStorage.setItem("tokenKey", response.access_token)
                        sessionStorage.setItem("userName", response.username)
                        $("#go-library-form").submit()
                    }
                },
            })
        },
        goBack() {
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

            $("#go-library-form").submit()
        },
    }
}

const app = Vue.createApp(AuthenticationForm)

var vm = app.mount('#authentication-form')