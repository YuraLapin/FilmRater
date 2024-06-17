const AuthenticationForm = {
    data() {
        return {
            login: "",
            password: "",
            loginRegister: "",
            password1: "",
            password2: "",
            regMode: showRegistration,
            logInError: wrongCridentialsError,
            logInErrorFill: false,
            regErrorPassword: false,
            regErrorTaken: loginTakenError,
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
        tryLogIn() {
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

            $("#input-login-username").val(vm.login)
            $("#input-login-password").val(vm.password)
            $("#input-login-camefrom").val(cameFrom)
            $("#input-login-filmid").val(passedFilmId)
            $("#try-login-form").submit()
        },
        tryRegister() {
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

            $("#input-register-username").val(vm.loginRegister)
            $("#input-register-password").val(vm.password1)
            $("#input-register-camefrom").val(cameFrom)
            $("#input-register-filmid").val(passedFilmId)
            $("#try-register-form").submit()
        }
    }
}

const app = Vue.createApp(AuthenticationForm)

var vm = app.mount('#authentication-form')