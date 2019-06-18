var Url = window.location.origin;
class Login extends React.Component {
    constructor(props) {
        super();
        this.state = {
            Email: "",
            UserPassword: "",
            Remember: "false",
        }
        this.handleChangeEmail = this.handleChangeEmail.bind(this);
        this.handleChangePassowrd = this.handleChangePassowrd.bind(this);
        this.handleChangeRemmber = this.handleChangeRemmber.bind(this);
        this.handleSubmitForm = this.handleSubmitForm.bind(this);
    }
    handleSubmitForm(event) {
        event.preventDefault();
        var temp = {
            Email: this.state.Email,
            Password: this.state.UserPassword,
            RememberMe: this.state.Remember
        }
        fetch("https://localhost:44343/AdminBCDH/Account/Login", {
            method: "POST",
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },

            body: JSON.stringify(temp)
        }).then(response => response.json()).then(response => Test(response));
    }
    handleChangeEmail(event) {

        let temp = event.target.value;
        this.setState({
            Email: temp
        });
    }
    handleChangePassowrd(event) {
        let temp = event.target.value;
        this.setState({
            UserPassword: temp
        });
    }
    handleChangeRemmber(event) {
        let temp = event.target.checked;
        if (temp) {
            this.setState({
                Remember: "True"
            });
        }
    }
    render() {
        return (
            <form onSubmit={event => this.handleSubmitForm(event)}>
                <div className="form-group">
                    <label>Tên đăng nhập</label>
                    <input type="email" className="form-control" placeholder="Email" onChange={e => this.handleChangeEmail(event)} />
                </div>
                <div className="form-group">
                    <label>Mật khẩu</label>
                    <input type="password" className="form-control" placeholder="Password" onChange={e => this.handleChangePassowrd(event)} required />
                </div>
                <div className="checkbox">
                    <label>
                        <input type="checkbox" onClick={e => this.handleChangeRemmber(event)} /> Ghi nhớ
                                </label>
                    <label className="pull-right">
                        <a href="#">Quên password?</a>
                    </label>
                </div>
                <button type="submit" className="btn btn-success btn-flat m-b-30 m-t-30">Đăng nhập</button>
            </form>
        );
    }
}
ReactDOM.render(<Login />, document.getElementById('FormLogin'));
function Test(sec) {
    var Error = sec.Error;
    var Status = sec.Status;

    if (Status == 0) {
        Swal.fire({
            title: 'Đăng nhập thành công',
            type: 'success',
            showConfirmButton: false,
            timer: 1500
        });
        setTimeout(function () {
            window.location.href = Url + "/AdminBCDH/Home";
        }, 2000);
    } else if (Status == 4 || Status==1) {
        Swal.fire({
            title: Error,
            type: 'error',
            showConfirmButton: true
        });
    }
    else {
        Swal.fire({
            title: 'Check mật khẩu hoặc tên đăng nhập!',
            type: 'error',
            showConfirmButton: true
        });
    }

}