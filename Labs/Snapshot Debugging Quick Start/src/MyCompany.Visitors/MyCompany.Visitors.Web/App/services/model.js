define(['config'], function (config) {
    var Visitor = function (entity) {
        var visitor = this;

        visitor.visitorId = entity && entity.VisitorId;
        visitor.firstName = entity && entity.FirstName;
        visitor.lastName = entity && entity.LastName;
        visitor.pictureContent = entity && entity.VisitorPictures && entity.VisitorPictures.length > 0 && entity.VisitorPictures[0].Content;
        visitor.company = entity && entity.Company;
        visitor.personalId = entity && entity.PersonalId;
        visitor.email = entity && entity.Email;
        visitor.position = entity && entity.Position;
        visitor.createdDateTime = entity && entity.CreatedDateTime;
        visitor.lastModifiedDateTime = entity && entity.LastModifiedDateTime;
        visitor.crmAccountManager = entity && entity.CrmAccountManager;
        visitor.crmLeads = entity && entity.CrmLeads;

        visitor.visits = [undefined, undefined, undefined, undefined];
        if (entity && entity.Visits && entity.Visits.length > 0) {
            for (var i = 0; i < entity.Visits.length; i++) {
                visitor.visits[i] = new Visit(entity.Visits[i]);
            }
        }
        visitor.lastVisit = entity && entity.LastVisit ? new Visit(entity.LastVisit, false) : new Visit(null, false);

        visitor = ko.mapping.fromJS(visitor);
        visitor = addVisitorComputeds(visitor);
        visitor = addVisitorValidations(visitor);

        return visitor;
    };

    var Visit = function (entity, includeVisitor) {
        var visit = this;

        visit.visitId = entity && entity.VisitId;
        visit.visitDateTime = (entity && entity.VisitDateTime) ? new Date(entity.VisitDateTime + '+00:00') : new Date();
        visit.createdDateTime = entity && entity.CreatedDateTime;
        visit.comments = entity && entity.Comments;
        visit.plate = entity && entity.Plate;
        visit.hasCar = entity && entity.HasCar;
        visit.visitorId = entity && entity.VisitorId;
        visit.employeeId = entity && entity.EmployeeId;

        visit.employee = new Employee(entity && entity.Employee);

        if (includeVisitor) {
            visit.visitor = ko.observable(new Visitor(entity && entity.Visitor));
        } else {
            visit.visitor = ko.observable({});
        }

        visit = ko.mapping.fromJS(visit);
        visit = addVisitComputeds(visit);
        visit = addVisitValidations(visit);

        return visit;
    };

    var Employee = function (entity) {
        var employee = this;

        employee.employeeId = entity && entity.EmployeeId;
        employee.firstName = entity && entity.FirstName;
        employee.lastName = entity && entity.LastName;
        employee.pictureContent = entity && entity.EmployeePictures && entity.EmployeePictures.length > 0 ? entity.EmployeePictures[0].Content : null;
        employee.position = entity && entity.JobTitle;
        employee.email = entity && entity.Email;

        return addEmployeeComputeds(ko.mapping.fromJS(employee));
    };

    var model = {
        Visitor: Visitor,
        Visit: Visit,
        Employee: Employee
    };

    return model;

    //#region Internal methods

    function addEmployeeComputeds(entity) {
        entity.fullName = ko.computed(function () {
            return ((entity.firstName() || '') + ' ' + (entity.lastName() || '')).trim();
        });
        entity.picture = ko.computed(function () {
            var content = entity.pictureContent();
            return content ? 'data:image/jpeg;base64,' + content : config.defaultPicture;
        });
        return entity;
    }

    function addVisitorComputeds(entity) {
        entity.fullName = ko.computed(function () {
            return (entity.firstName() || '') + ' ' + (entity.lastName() || '');
        });
        entity.picture = ko.computed(function () {
            var content = entity.pictureContent();
            return content ? 'data:image/jpeg;base64,' + content : config.defaultPicture;
        });
        entity.nextVisit = ko.computed(function () {
            if (entity.visits()[0]) {
                return entity.visits()[0];
            } else {
                return new Visit();
            }
        });
        return entity;
    }

    function addVisitComputeds(entity) {
        entity.displayDate = ko.computed(function () {
            if (!entity.visitId()) return ' - ';

            var date = moment.utc(entity.visitDateTime().toUTCString());
            return entity.visitDateTime() && date.isValid() ? date.local().format(config.dateFormat) : ' - ';
        });
        entity.displayTime = ko.computed(function () {
            if (!entity.visitId()) return ' - ';

            var date = moment.utc(entity.visitDateTime().toUTCString());
            return entity.visitDateTime() && date.isValid() ? date.local().format(config.timeFormat) : ' - ';
        });
        return entity;
    }

    function addVisitorValidations(entity) {
        entity.firstName.extend({ required: true });
        entity.lastName.extend({ required: true });
        entity.company.extend({ required: true });
        entity.email.extend({ required: true, email: true });

        entity["errors"] = ko.validation.group(entity); // ko.validatedObservable()

        return entity;
    }

    function addVisitValidations(entity) {
        entity.visitDateTime.extend({ date: true });
        entity.visitorId.extend({ required: true });

        entity["errors"] = ko.validation.group(entity);
        return entity;
    }

    //#endregion
});