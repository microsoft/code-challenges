define(['services/model', 'services/dataservice', 'services/enums', 'viewmodels/base', 'durandal/system'],
    function (model, dataservice, enums, base, system) {
        var steps = {
            selectBigSize: { id: 1, minSelectionWidth: 280, minSelectionHeight: 380 },
            selectSmallSize: { id: 2, minSelectionWidth: 100, minSelectionHeight: 100, maxSelectionWidth: 200, maxSelectionHeight: 200 }
        };

        var image = ko.observable(),
            title = ko.observable(),
            actionText = ko.observable(),
            step,
            bigSelection,
            cropControl,
            $image;

        var vm = {
            activate: activate,
            viewAttached: viewAttached,
            title: title,
            actionText: actionText,
            close: close,
            accept: accept,
            image: image,
            imageLoaded: imageLoaded
        };

        return vm;

        function activate(routeData) {
            if (routeData && routeData.picture) {
                step = steps.selectBigSize;
                title('1/2 Select your big size image');
                actionText('Next');
                image(routeData.picture);
            } else {
                // show error message
            }
        };

        function viewAttached() {
            $image = $('.crop');

            setTimeout(function () {
                var coords = getInitialCoords();

                cropControl = $image.imgAreaSelect({
                    aspectRatio: step.minSelectionWidth + ':' + step.minSelectionHeight,
                    minWidth: step.minSelectionWidth,
                    minHeight: step.minSelectionHeight,
                    handles: true,
                    x1: coords.x1,
                    y1: coords.y1,
                    x2: coords.x2,
                    y2: coords.y2,
                    instance: true,
                    parent: $('.modal-crop')
                });
            }, 200);
        }

        function imageLoaded() {
            // centering the modal window after image was loaded
            var $modal = $('.modal-crop').parent();
            var width = $modal.width();
            var height = $modal.height();
            $modal.css({
                'margin-top': (-height / 2).toString() + 'px',
                'margin-left': (-width / 2).toString() + 'px'
            });
        }

        function close() {
            vm.modal.close();
        }

        function accept() {
            if (step.id == steps.selectBigSize.id) {
                bigSelection = cropControl.getSelection();
                step = steps.selectSmallSize;
                
                var coords = getInitialCoords();
                cropControl.setOptions({
                    aspectRatio: step.minSelectionWidth + ':' + step.minSelectionHeight,
                    minWidth: step.minSelectionWidth,
                    minHeight: step.minSelectionHeight,
                    maxWidth: step.maxSelectionWidth,
                    maxHeight: step.maxSelectionHeight,
                    x1: coords.x1,
                    y1: coords.y1,
                    x2: coords.x2,
                    y2: coords.y2,
                });
                cropControl.update();

                title('2/2 Select your small size image');
                actionText('Finish');
                $image.hide().fadeIn();
            }
            else if (step.id == steps.selectSmallSize.id) {
                var w = $image.width();
                var h = $image.height();
                vm.modal.close({ bigSelection: bigSelection, smallSelection: cropControl.getSelection(), w: w, h: h });
            }
        }

        function getInitialCoords() {
            var width = $image.innerWidth(),
                height = $image.innerHeight(),
                x1, x2, y1, y2;

            x1 = (width / 2) - (step.minSelectionWidth / 2);
            x2 = width > step.minSelectionWidth ? x1 + step.minSelectionWidth : width;
            y1 = (height / 2) - (step.minSelectionHeight / 2);
            y2 = height > step.minSelectionHeight ? y1 + step.minSelectionHeight : height;

            return {
                x1: x1,
                x2: x2,
                y1: y1,
                y2: y2
            };
        };
    });