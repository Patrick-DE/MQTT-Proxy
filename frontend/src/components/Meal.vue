<template>
    <div>
      <div class="jumbotron jumbotron-fluid">
        <div class="container">
          <h1>Bestellungen</h1>
          <p>
            Hier findet ihr die aktuelle Speisekarte und eine Liste aller Bestellungen.<br>
            Informiert euch vor der Bestellung, was alles auf der Speisekarte steht - dann geht es mit der Bestellaufnahme gleich schneller!
          </p>
        </div>
      </div>
        <div class="container">
          <b-tabs content-class="mt-3">
            <b-tab title="Speisekarte" active>
              <MealMenu :menu="this.menu"></MealMenu>
            </b-tab>
            <b-tab title="Bestellungen">
              <MealOrdersList
                :menu="this.menu"
                :tables="this.tables"
                :orders="this.orders">
                </MealOrdersList>
            </b-tab>
            <b-tab title="Neue Bestellung" v-if="user && user.role == 'admin'">
              <MealOrderNew>
                :menu="this.menu"
                :tables="this.tables"
                :orders="this.orders">
                </MealOrderNew>
            </b-tab>
          </b-tabs>
        </div>
    </div>
</template>

<script>
import Alert from './Alert'
import MealMenu from './MealComponents/MealMenu'
import MealOrdersList from './MealComponents/MealOrdersList'
import MealOrderNew from './MealComponents/MealOrderNew'

const MENU = {
    "sizes": [
        {
            "size": "Normal (27cm)",
            "price": 6
        }
    ],
    "meals": [
        {
            "id": "06",
            "name": "Vegetaria",
            "description": "Artischocken, Zwiebeln, Paprika, Peperoni"
        }
    ]
};
const TABLES =  [
    {
      "name": "ORGA",
      "seats": 10
    }
  ];

const ORDERS = [
      {
        "id": "1",
        "table": "ORGA",
        "seat": 0,
        "meal": "36",
        "size": "Normal (27cm)",
        "delivered": false
      }
    ];

export default {
  name: 'Meal',
  props:['user'],
  data () {
    return {
        //menu: null,
        //tables: null,
        //orders: null,
        menu: MENU,
        tables: TABLES,
        orders: ORDERS,
        'formatter': new Intl.NumberFormat('de-DE', {
            style: 'currency',
            currency: 'EUR',
            minimumFractionDigits: 2
        })
    }
  },
  components: { 
    MealMenu, MealOrdersList, MealOrderNew
  },
  created: function() {
    this.getMealInfo();
  },
  methods: {
    formatError: function(error) {
      if (!error) {
        return "";
      } else {
        if (error.response) {
          if (error.response.data) {
            return `${error.response.data} (${error.response.status} - ${error.response.statusText})`;
          }
          return `${error.response.status} - ${error.response.statusText}`;
        } else {
          return "Keine Antwort."
        }
      }
    },
    getTableSizes: function(){
      if (!this.menu || !this.menu.sizes || !this.menu.sizes.length) return [];
      return arr.map(s=>{return { Größe: s.size, Preis: this.formatter.format(s.price) };});
    },
    getMealInfo: function(event) {
      this.axios
        .get('/meals/fullinfo')
        .then(res => {
          this.menu = res.data.menu;
          this.tables = res.data.tables;
          this.orders = res.data.orders;
        });
    },
    deleteOrders: function(event) {
      var orders = this.info.orders.filter(x => x.selected || false);
      for (var i = 0; i < orders.length; i++) {
        this.axios
          .delete(this.getEndpoint(`orders/${orders[i].id}`))
          .then(res => {
            this.info.orders = res.data;
          }).catch((error) => {
            console.warn(error);
            this.$refs.alertRemove.show('danger','Fehler!', this.formatError(error));
          })
      }
      this.showButton = false;
    },
    newOrder: function(event) {
      var order = event.order;
      this.axios
        .post(this.getEndpoint(), order)
        .then(res => {
          this.info = {orders: res.data};
        }).catch((error) => {
          this.$refs.alertAdd.show('danger','Fehler!', this.formatError(error));
        });
    }
  }
}
</script>