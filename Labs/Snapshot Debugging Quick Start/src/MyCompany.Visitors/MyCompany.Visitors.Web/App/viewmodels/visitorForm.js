define(['services/model', 'services/dataservice', 'services/enums', 'viewmodels/base', 'durandal/app', 'config'],
    function (model, dataservice, enums, base, app, config) {
        var modes = {
            add: 1,
            update: 2
        };
        
        var visitorEntity = ko.observable(),
            file = ko.observable(),
            picture = ko.observable(),
            title = ko.observable(),
            actionText = ko.observable(),
            imageButtonText = ko.computed(function () {
                return picture() ? 'Change image' : 'Add image';
            }),
            mode;

        var vm = {
            title: title,
            actionText: actionText,
            activate: activate,
            visitorEntity: visitorEntity,
            close: close,
            accept: accept,
            file: file,
            picture: picture,
            imageChanged: imageChanged,
            cropSelection: null,
            imageButtonText: imageButtonText
        };

        return vm;

        function activate(routeData) {
            file(null);
            picture(null);
            vm.cropSelection = null;

            if (routeData && routeData.visitor) {
                mode = modes.update;
                title('Edit visitor');
                actionText('Save changes');
                visitorEntity(routeData.visitor);

                base.showLoading();
                dataservice.getVisitorPicture(routeData.visitor.visitorId(), enums.pictureType.big).then(function (data) {
                    if (data) {
                        picture(data);
                    } else {
                        picture(config.defaultBigPicture);
                    }
                    base.hideLoading();
                });
            } else {
                mode = modes.add;
                title('New visitor');
                actionText('Save changes');
                visitorEntity(new model.Visitor());
            }
        };

        function close() {
            vm.modal.close();
        }

        function accept() {
            if (!visitorEntity().isValid()) {
                visitorEntity().errors.showAllMessages(true);
                return;
            }

            if (mode == modes.add) {
                base.showLoading();
                dataservice.addVisitor(visitorEntity, { data: file(), selection: vm.cropSelection }).then(function (data) {
                    base.hideLoading();
                    vm.modal.close(data);
                });
            }
            else if (mode == modes.update) {
                base.showLoading();
                dataservice.updateVisitor(visitorEntity, { data: file(), selection: vm.cropSelection }).then(function (data) {
                    base.hideLoading();
                    vm.modal.close(data);
                });
            }
        }
        
        function imageChanged (image) {
            app.showModal('viewmodels/cropImageWizard', { picture: image }).then(function (data) {
                if (data) {
                    vm.cropSelection = data;
                    picture(image);
                } else {
                    vm.cropSelection = null;
                }
            });
        }
    });