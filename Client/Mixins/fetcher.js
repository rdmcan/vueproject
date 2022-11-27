/* INFO-3067 -  
Student Name: Ruben Dario Mejia Cardona
Student Number:0864646
Brief: Define the objects and methods used by mixin*/


// define a mixin object
export default {
    data() {
        return {
            SERVERURL: `https://localhost:44385/api/`
        };
    },
    methods: {
           $_getdata: async function (apicall) {
            let payload = {};
            let headers = this.buildHeaders();
            try {
                let response = await fetch(`${this.SERVERURL}${apicall}`, {
                    method: "GET",
                    headers: headers
                });
                payload = await response.json();
                console.log(payload);
            } catch (err) {
                console.log(err);
                payload = err;
            }
            return payload;
        },
        // don't use arrow function here
        $_postdata: async function (apicall, data) {
            let headers = {};
            let payload = JSON.stringify(data);
            if (apicall === "register" || apicall === "login") {
                headers = { "Content-Type": "application/json; charset=utf-8" };
            } else {
                headers = this.buildHeaders();
            }
            try {
                let response = await fetch(`${this.SERVERURL}${apicall}`, {
                    method: "POST",
                    headers: headers,
                    body: payload
                });
                payload = await response.json();
            } catch (error) {
                payload = error;
            }
            return payload;
        },
        buildHeaders: function () {
            const myHeaders = new Headers();
            const customer = JSON.parse(sessionStorage.getItem("customer"));
            myHeaders.append("Content-Type", "application/json");
            myHeaders.append("Authorization", "Bearer " + customer.token);
            return myHeaders;
        }
     //Allows communication between server and client after the HTTP Authorization
    }
};	