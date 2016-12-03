function TaskViewModel() {
    this.taskId = null;
    this.title = null;
    this.tagId = null;
    this.tagName = null;
    this.estimatedDifficulty = null;
    this.description = null;
    this.estimatedCompletionDate = null;
}

var TasksViewModel = new Vue({
    el: "#task-view-model",
    data: {
        tableCaption: "",
        tasks: []
    },
    methods: {

    }
});