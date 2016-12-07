function TaskViewModel(title) {
    this.taskId = null;
    this.title = title;
    this.tagId = null;
    this.tagName = null;
    this.estimatedDifficulty = null;
    this.description = null;
    this.created = Date.now();
    this.estimatedCompletionDate = Date.now();
};

function daysDiff(fstDate, secDate) {
    var date1 = new Date(fstDate);
    var date2 = new Date(secDate);
    var timeDiff = Math.abs(date2.getTime() - date1.getTime());
    var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24));
    return diffDays;
};


var app = new Vue({
    el: '#app-task-manager',
    data: {
        age: 0,
        title: null,
        tableCaption: null,
        tasks: [],
        taskCounter: 0
    },
    methods:{
        loadTasks: function () {
            this.tasks = [new TaskViewModel("New Task1"), new TaskViewModel("New Task2"), new TaskViewModel("New Task3"), new TaskViewModel("New Task4")]
        },
        incCounter: function () {
            app.taskCounter++;
        },
        decCounter: function () {
            if (app.taskCounter > 0) {
                app.taskCounter--;
            }
        }
    },
    computed:{
        completePercent: function () {
            if (this.taskCounter !== 0 && this.tasks.length != 0) {
                return Math.round(this.taskCounter / this.tasks.length * 100);
            }
            return 0;
        }
    },
    mounted: function(){
        this.loadTasks();
        
    },
    components: {
        'task-row': {
            template: '#task-row-template',
            props: ['task'],
            data: function(){
                return {
                    isChecked: false
                }
            },
            watch: {
                isChecked: function (check) {
                    if (check) {
                        this.$parent.$options.methods.incCounter();
                    } else {
                        this.$parent.$options.methods.decCounter();
                    }
                }
            },
            computed: {
                title: function () {
                    return this.task.title;
                },
                createdOn: function () {
                    return new Date(this.task.created);
                },
                estDate: function () {
                    new Date(this.task.estimatedCompletionDate);
                },
                daysRemain: function () {
                    return daysDiff(this.task.created, this.task.estimatedCompletionDate);
                }
            }
        }
    }
});