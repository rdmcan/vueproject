<!-- INFO-3067 -  
Student Name: Ruben Dario Mejia Cardona
Student Number:0864646
Brief: Display the brand and products. Customers can select a product to add to the cart  -->

<template>
  <v-container fluid>
    <v-row justify="center" class="text-center display-2 indigo--text" style="margin-top:.5em;">Brands</v-row>
    <v-row justify="center">
      <v-col class="title text-center" style="color:teal">{{status}} </v-col>
      </v-row>
       <v-col justify="center" align="center">
        <v-img
                  :src="require('../assets/musicstore.png')"
                  style="height:10vh;width:10vh;"
                  aspect-ratio="1"
                />
      </v-col>
     <v-row justify="center">
      <v-col class="text-left display-1">
        <v-select
          :items="brands"
          item-text="name"
          style="max-height: 50%;"
          item-value="id"
          @input="changeBrand" 
          v-model="selectedid"
        ></v-select>
      </v-col>
    </v-row>
    <div v-if="products.length > 0">
      <v-row justify="center" class="title text-center headline indigo--text ">Products in Brand</v-row>
      <v-row justify="center" style="margin-top:1vh;">
        <v-col class="text-left display-2">
          <v-list style="max-height: 60vh;" class="overflow-y-auto">
            <v-list-item-group>
              <v-list-item
                v-for="(product, p) in products"
                :key="p"
                style="border:solid;"
                @click="popDialog(product)"
              >
                <v-col style="width:25%;">
                  <v-img
                    v-if="product.graphicName"
                    :src="require(`../assets/${product.graphicName}`)"
                    class="my-3"
                    style="height:10vh;width:10vh;"
                    aspect-ratio="1"
                  />
                </v-col>
                <v-col style="width:75%;">
                  <v-list-item-content>
                    <v-list-item-title class="title teal--text" v-text="product.productName">></v-list-item-title>
                  </v-list-item-content>
                </v-col>
              </v-list-item>
            </v-list-item-group>
          </v-list>
        </v-col>
      </v-row>
    </div>
    <v-dialog v-model="dialog" v-if="selectedProduct" justify="center" align="center">
      <v-card>
        <v-row>
          <v-spacer></v-spacer>
          <v-btn text @click="dialog = false" class="indigo--text" style="font-size:XX-large;margin:1vw;">X</v-btn>
        </v-row>
        <v-row
          class="title"
          justify="center"
          align="center"
          style="margin-left:3vw;margin-right:3vw;"
        >{{selectedProduct.productName}}</v-row>
        <v-row>
          <v-img 
            v-if="selectedProduct.graphicName"
            :src="require(`../assets/${selectedProduct.graphicName}`)"
            class="my-3"
            style="height:25vh;width:25vh;"
            aspect-ratio="1"
          /> <!-- Add the image from the source file to display every product -->
        </v-row>
        <v-row>
          <v-col class="title" style="margin-left:5vw;margin-right:5vw;">MSRP:</v-col>
          <v-col>{{selectedProduct.msrp | currency}}</v-col>
        </v-row>
        <v-row>
          <v-col class="title" style="margin-left:5vw;margin-right:5vw;">Description:</v-col>
        </v-row>
        <v-row>
          <v-row
            class="text"
            justify="center"
            align="center"
            style="margin-left:5vw;margin-right:5vw;"
          >{{selectedProduct.description}}</v-row>
          <v-col></v-col>
          <v-col></v-col>
        </v-row>
        <v-row style="margin-left:3vw;">
          <v-col>Qty:</v-col>
          <v-col>
            <input
              type="number"
              maxlength="3"
              placeholder="enter qty"
              size="3"
              style="width: 15vw;border-bottom:solid;text-align:right"
              v-model="qty"
            />
          </v-col>
          <v-col cols="7"></v-col>
        </v-row>
        <v-row justify="center" align="center" style="margin-bottom:2vh;margin-left:3vw;">
          <v-col>
            <v-btn class="indigo darken-4 white--text" @click="addToCart">Add To Cart</v-btn>
          </v-col>
          <v-col>
            <v-btn class="indigo darken-4 white--text" @click="viewCart">View Cart</v-btn>
          </v-col>
        </v-row>
        <v-row justify="center" align="center" style="padding-bottom:5vh;">{{this.dialogStatus}}</v-row>
      </v-card>
    </v-dialog>
  </v-container>
</template>
<script>
import fetcher from "../mixins/fetcher";
export default {
  name: "BrandList",
  data() {
    return {
      brands: [],
      status: {},
      selectedid: 0,
      products: [],
      dialog: false,
      selectedProduct: {},
      dialogStatus: "",
      qty: 0,
      cart: []
    };
  },
  mixins: [fetcher],
  mounted: async function() {
    // don't use arrow function here
    try {
      this.status = "fetching brands from server...";
      this.brands = await this.$_getdata("brand"); // $_getdata is in mixin
      this.status = `loaded ${this.brands.length + 1} brands`;
      this.brands.unshift({ name: "Current Brands", id: 0 });
    } catch (err) {
      console.log(err);
      this.status = `Error has occured: ${err.message}`;
    }
    if (sessionStorage.getItem("cart")) {
      this.cart = await JSON.parse(sessionStorage.getItem("cart"));
    }
  },
  methods: {
    changeBrand: async function(brandid) {
      if (brandid > 0) {
        // don't use arrow function here
        try {
          this.status = `fetching products for ${brandid}...`;
          this.products = await this.$_getdata(`product/${brandid}`);
          this.status = `found ${this.products.length} products`;
        } catch (err) {
          console.log(err);
          this.status = `Error has occured: ${err.message}`;
        }
      }
    },
    popDialog: function(product) {
      this.dialogStatus = "";
      this.dialog = !this.dialog;
      this.selectedProduct = product;
    },
   addToCart: function() {
      const index = this.cart.findIndex(
        // is product already on the cart
        product => product.id === this.selectedProduct.id
      );
      if (this.qty !== "0") {
        index === -1
          ? this.cart.push({
              id: this.selectedProduct.id,
              qty: parseInt (this.qty),
              product: this.selectedProduct
            }) // add
          : (this.cart[index] = {
              // replace
              id: this.selectedProduct.id,
              qty: parseInt (this.qty),
              product: this.selectedProduct
            });
        this.dialogStatus = `${this.qty} product (s) added`;
      } else {
        index === -1 ? null : this.cart.splice(index, 1); // remove
        this.dialogStatus = `product(s) removed`;
      }
      sessionStorage.setItem("cart", JSON.stringify(this.cart));
    },
    viewCart: function() {
      this.$router.push({
        name: "cart"
      });
    }
  }
};
</script>
