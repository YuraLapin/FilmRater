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
                $("#input-login-username").val(vm.login)
                $("#input-login-password").val(vm.password)
                $("#input-login-camefrom").val(cameFrom)
                $("#input-login-filmid").val(passedFilmId)
                $("#try-login-form").submit()
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
                $("#input-register-username").val(vm.loginRegister)
                $("#input-register-password").val(vm.password1)
                $("#input-register-camefrom").val(cameFrom)
                $("#input-register-filmid").val(passedFilmId)
                $("#try-register-form").submit()
            }
        }
    }
}

const app = Vue.createApp(AuthenticationForm)

var vm = app.mount('#authentication-form')