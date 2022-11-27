<!-- INFO-3067 -  
Student Name: Ruben Dario Mejia Cardona
Student Number:0864646
Brief: Display the Cart detais and sent the order info to the server  -->

<template>
  <v-container>
    <v-row justify="center">
      <v-col class="display-1 text-center indigo--text">Cart Contents</v-col>
    </v-row>
    <v-row>
      <v-col justify="center" align="center">
        <v-img
          :src="require('../assets/cart.png')"
          style="height:8vh;width:12vh;"
          aspect-ratio="1"
        /><!-- Add the image from the source -->
      </v-col>
    </v-row>
    <v-row style="margin:1vw;" justify="center" class="title indigo--text">{{status}}</v-row>
    <div v-if="cart.length > 0">
      <v-simple-table>
        <thead>
          <tr>
            <th class="text-left title indigo--text">Id</th>
            <th class="text-left title indigo--text">Name</th>
            <th class="text-left title indigo--text">Qty</th>
            <th class="text-left title indigo--text">Price</th>
            <th class="text-left title indigo--text">Ext</th>
          </tr>
        </thead>
      </v-simple-table>
      <v-simple-table style="max-height: 15vh;margin-bottom:8vh;" class="overflow-y-auto">
        <tbody>
          <tr v-for="cartProduct in cart" :key="cartProduct.id">
            <td>{{ cartProduct.id }}</td>
            <td>{{ cartProduct.product.productName }}</td>
            <td>{{ cartProduct.qty }}</td>
            <td>{{ cartProduct.product.msrp | currency }}</td>
            <td>{{ cartProduct.product.msrp * cartProduct.qty | currency }}</td>
          </tr>
        </tbody>
      </v-simple-table>
      <v-simple-table>
        <thead>
          <th colspan="2" class="text" justify="right" align="right" style="border-top: 2px solid #1A237E;
            margin-left:5vw;margin-right:5vw;"></th>
          </thead>
        <tbody>
          <tr class="text" justify="right" align="right" style="margin-left:5vw;margin-right:5vw;">
            <td><strong>Sub-Total:</strong></td>
            <td>{{subTot | currency}}</td>
          </tr>
          <tr class="text" justify="right" align="right" style="margin-left:5vw;margin-right:5vw;">
            <td><strong>Tax:</strong></td>
            <td>{{taxTot | currency}}</td>
          </tr>
          <tr class="text" justify="right" align="right" style="margin-left:5vw;margin-right:5vw;">
            <td><strong>Cart Total:</strong></td>
            <td>{{cartTot | currency}}</td>
          </tr>
        </tbody>
      </v-simple-table>
      <v-row  justify="center" align="center" style="margin-bottom:2vh;margin-left:3vw;">
        <v-col>
        <v-btn
          color="indigo darken-4 white--text"
          @click="clearCart"
        >Empty Cart</v-btn>
        </v-col>
        <v-col>
        <v-btn
          color="indigo darken-4 white--text"
          @click="addOrder"
        >Save Order</v-btn>
        </v-col>
      </v-row>
    </div>
  </v-container>
</template>
<script>
import fetcher from "../mixins/fetcher";
export default {
  name: "CartDetails",
  data() {
    return {
      subTot: 0,
      taxTot: 0,
      cartTot: 0,
      cart: [],
      status: ""
    };
  },
  beforeMount: function() {
    if (sessionStorage.getItem("cart")) {
      this.cart = JSON.parse(sessionStorage.getItem("cart"));
      this.cart.map(cartProduct => {
        this.subTot += cartProduct.product.msrp * cartProduct.qty;
        this.taxTot += cartProduct.product.msrp * cartProduct.qty * 0.13;
        this.cartTot +=
          cartProduct.product.msrp * cartProduct.qty * 0.13 +
          cartProduct.product.msrp * cartProduct.qty;
      });
    } else {
      this.cart = [];
    }
  },
  mixins: [fetcher],
  methods: {
    clearCart: function() {
      sessionStorage.removeItem("cart");
      this.cart = [];
      this.status = "cart cleared";
    },
    addOrder: async function() {
      let customer = JSON.parse(sessionStorage.getItem("customer"));
      let cart = JSON.parse(sessionStorage.getItem("cart"));

      try {
        this.status = "sending cart info to server";
        let orderHelper = { email: customer.email, selections: cart };
        let payload = await this.$_postdata("order", orderHelper); // in mixin

        if (payload.indexOf("not") > 0) {
          this.status = payload;
        } else {
          this.clearCart(), (this.status = payload);
        }
      } catch (err) {
        console.log(err);
        this.status = `Error add order: ${err}`;
      }
    }
  }
};
</script>