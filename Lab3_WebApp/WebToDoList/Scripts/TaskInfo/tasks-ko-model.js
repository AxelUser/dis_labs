function TaskViewModel() {
    var self = this;

    self.tasks = ko.observableArray();
    self.tags = ko.observableArray();

    self.pullTasksData = function (apiTaskUrl) {
        $.get(apiTaskUrl, function (tasksModels) {
            self.tasks(tasksModels);
        });
    }

    self.pullTagsData = function (apiTagsUrl) {
        $.get(apiTagsUrl, function (tagsViewModels) {
            self.tags(tagsViewModels);
        });
    }
}

ko.applyAllBindings(new TaskViewModel());