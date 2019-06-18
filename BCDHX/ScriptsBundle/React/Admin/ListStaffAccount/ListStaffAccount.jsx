
class ListStaffAccount extends React.Component {
    constructor(props) {
        super();
        this.State = {
            items: []
        }
    }
    GetListFormController() {
        fetch("https://localhost:44343/AdminBCDH/ProcessGetResources/GetListStaffAccount", {
            method: "GET",
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
        }).then(response => response.json()).then(newItem => this.setState((prevState, props) => ({
            items: [...prevState.items, newItem]
        })
        )
        )
    }
    render() {
        return (
            <DataGrid
                dataSource={this.State.items}
                columns={['CompanyName', 'City', 'State', 'Phone', 'Fax']}
                showBorders={true}
            />
        );
    }
}
ReactDOM.render(
    <ListStaffAccount />,
    document.getElementById('ListStaff')
);