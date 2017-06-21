ko.bindingHandlers.uploadFile = {
    _preview: function preview($container, src, selection) {
        var image = new Image();
        image.onload = function () {
            if (selection && selection.bigSelection) {
                var cropSelection = selection.bigSelection;
                var previewWidth = $container.width();
                var previewHeight = $container.height();

                var scaleX = previewWidth / (cropSelection.width || 1);
                var scaleY = previewHeight / (cropSelection.height || 1);
                var width = Math.round(scaleX * selection.w) + 'px';
                var height = Math.round(scaleY * selection.h) + 'px';
                var marginLeft = '-' + Math.round(scaleX * cropSelection.x1) + 'px';
                var marginTop = '-' + Math.round(scaleY * cropSelection.y1) + 'px';

                image.style.width = width;
                image.style.height = height;
                image.style.marginLeft = marginLeft;
                image.style.marginTop = marginTop;
                image.style.maxWidth = 'none';
            } else {
                image.style.maxWidth = '100%';
            }
        };
        image.src = src;
        $container.find('img').remove();
        $container.append(image);
    },
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var previewElement = document.createElement("div");
        previewElement.className = "image-preview";
        element.parentNode.parentNode.insertBefore(previewElement, element.parentNode);

        var binding = valueAccessor();
        var fileReader = new FileReader();

        fileReader.onload = function (event) {
            binding.onChanged(event.target.result);
        };

        element.addEventListener("change", updateImagePreview, false);
        updateImagePreview();

        function updateImagePreview() {
            var file = element.files[0];
            if (file) {
                fileReader.readAsDataURL(file);
                binding.file(file);
            } else {
                var pictureSrc = binding.picture();
                if (pictureSrc) {
                    ko.bindingHandlers.uploadFile._preview($(previewElement), pictureSrc);
                } else {
                    var placeholderSrc = element.getAttribute("data-placeholder");
                    if (placeholderSrc) {
                        ko.bindingHandlers.uploadFile._preview($(previewElement), placeholderSrc);
                    } else {
                        previewElement.removeAttribute("src");
                    }
                }
            }
        }
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var $previewElement = $('.image-preview');
        if ($previewElement) {
            var image = valueAccessor().picture();
            ko.bindingHandlers.uploadFile._preview($previewElement, image, viewModel.cropSelection);
        }
    }
};

ko.bindingHandlers.dateTimePicker = {
    _populate: function (element, property, unit) {
        var currentDate = moment();
        var selectedDate = moment.utc(property()).local();
        var options = [];

        if (unit === 'year') {
            var currentYear = currentDate.year();
            var years = 5;
            for (var y = 0; y <= years; y++) {
                var year = { value: currentYear + y };
                if (year.value == selectedDate.year()) {
                    year.selected = true;
                }
                options.push(year);
            }
        } else if (unit === 'month') {
            var months = 12;
            var monthsTexts = moment().lang()._months;
            var isSameYear = selectedDate.isSame(currentDate, 'year');
            var updateMonth = false;
            for (var i = 0; i < months; i++) {
                if (isSameYear && i < currentDate.month()) {
                    updateMonth = true;
                    continue;
                }

                if (updateMonth) {
                    if (i > selectedDate.month()) {
                        var date = selectedDate.toDate();
                        date.setMonth(i);
                        property(date);
                    }
                    updateMonth = false;
                }

                var month = { value: i, text: monthsTexts[i] };
                if (month.value == selectedDate.month()) {
                    month.selected = true;
                }
                options.push(month);
            }
        } else if (unit === 'day') {
            var days = selectedDate.daysInMonth();
            var isSameMonth = selectedDate.isSame(currentDate, 'month');
            var updateDay = false;
            for (var d = 1; d <= days; d++) {
                if (isSameMonth && d < currentDate.date()) {
                    updateDay = true;
                    continue;
                }

                if (updateDay) {
                    if (d > selectedDate.date()) {
                        var date = selectedDate.toDate();
                        date.setDate(d);
                        property(date);
                    }
                    updateDay = false;
                }

                var day = { value: d, showTwoDigits: true };
                if (day.value == selectedDate.date()) {
                    day.selected = true;
                }
                options.push(day);
            }
        } else if (unit === 'hour') {
            var hours = 24;
            var isSameDay = selectedDate.isSame(currentDate, 'day');
            var updateHour = false;
            for (var h = 0; h < hours; h++) {
                if (isSameDay && h < currentDate.hours()) {
                    updateHour = true;
                    continue;
                }

                if (updateHour) {
                    if (h > selectedDate.hours()) {
                        var date = selectedDate.toDate();
                        date.setHours(h);
                        property(date);
                    }
                    updateHour = false;
                }

                var hour = { value: h, showTwoDigits: true };
                if (hour.value == selectedDate.hours()) {
                    hour.selected = true;
                }
                options.push(hour);
            }
        } else if (unit === 'minute') {
            var minutes = 60;
            var isSameHour = selectedDate.isSame(currentDate, 'hour');
            var updateMinute = false;
            for (var m = 0; m < minutes; m++) {
                if (isSameHour && m < currentDate.minutes()) {
                    updateMinute = true;
                    continue;
                }

                if (updateMinute) {
                    if (m > selectedDate.minutes()) {
                        var date = selectedDate.toDate();
                        date.setMinutes(m);
                        property(date);
                    }
                    updateMinute = false;
                }

                var minute = { value: m, showTwoDigits: true };
                if (minute.value == selectedDate.minutes()) {
                    minute.selected = true;
                }
                options.push(minute);
            }
        }

        $(element).children().each(function () {
            $(this).remove();
        });

        options.forEach(function (item, index) {
            var option = document.createElement('option');
            if (item.showTwoDigits) {
                option.innerHTML = ("0" + (item.text || item.value)).slice(-2);
            } else {
                option.innerHTML = item.text || item.value;
            }
            option.value = item.value;
            option.selected = item.selected || false;

            element.appendChild(option);
        });
    },
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var binding = valueAccessor();

        ko.bindingHandlers.dateTimePicker._populate(element, binding.property, binding.unit);

        $(element).change(function () {
            var $this = $(this);
            var val = parseInt($this.val());
            var date = binding.property();

            if (binding.unit == 'year') {
                date.setYear(val);
            }
            else if (binding.unit == 'month') {
                date.setMonth(val);
            }
            else if (binding.unit == 'day') {
                date.setDate(val);
            }
            else if (binding.unit == 'hour') {
                date.setHours(val);
            }
            else if (binding.unit == 'minute') {
                date.setMinutes(val);
            }

            binding.property(date);
        });
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var binding = valueAccessor();

        ko.bindingHandlers.dateTimePicker._populate(element, binding.property, binding.unit);
    }
};