<template>
    <div v-if="active" :class="completeClass" role="alert">
        <strong>{{title}}</strong> {{text}}
        <button type="button" class="close" v-on:click="active=false" aria-label="Close">
            <span aria-hidden="true">&times;</span>
        </button>
    </div>
</template>

<script>
export default {
    name:'Alert',
    data: function() {
        return {
            active: false,
            title: "Dummy",
            text: "Dummy",
            completeClass: `alert alert-warning alert-dismissible fade show`
        }
    },
    methods: {
        show: function(_class, _title, _text) {
            this.active = true;
            this.completeClass = `alert alert-${_class} alert-dismissible fade show`;
            this.title = _title;
            this.text = _text;
        }
    }
}
</script>
