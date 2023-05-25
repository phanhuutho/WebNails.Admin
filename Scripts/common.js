function ResultDate(dateInput)
{
    var miliseconds = dateInput.replace("/Date(", "").replace(")/", "");
    var getDate = new Date(parseInt(miliseconds));
    var dd = getDate.getDate();
    var mm = getDate.getMonth() + 1; //January is 0!

    var yyyy = getDate.getFullYear();
    if (dd < 10) {
        dd = "0" + dd;
    }
    if (mm < 10) {
        mm = "0" + mm;
    }
    return dd + "/" + mm + "/" + yyyy;
}

function ResultDateCountDown(dateInput)
{
    var miliseconds = dateInput.replace("/Date(", "").replace(")/", "");
    var getDate = new Date(parseInt(miliseconds));
    var dd = getDate.getDate();
    var mm = getDate.getMonth() + 1; //January is 0!

    var yyyy = getDate.getFullYear();
    if (dd < 10) {
        dd = "0" + dd;
    }
    if (mm < 10) {
        mm = "0" + mm;
    }
    return yyyy + "/" + mm + "/" + dd;
}

function getUrlParameter(sParam) {
    var sPageUrl = decodeURIComponent(window.location.search.substring(1)),
        sUrlVariables = sPageUrl.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sUrlVariables.length; i++) {
        sParameterName = sUrlVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : sParameterName[1];
        }
    }
    return null;
}

function RedirectSortView(valueSort, nameController, nameArea)
{
    var path = location.pathname.split('/');
    var actionName = path[4] != null ? path[4] : path[3];
    var valueSortBy = getUrlParameter("SortBy");
    var valueOrderBy = getUrlParameter("OrderBy");
    var valueCountSort = getUrlParameter("CountSort");
    var valuePage = getUrlParameter("page");
    var pathname = window.location.search.replace("&SortBy=" + valueSortBy, "").replace("?SortBy=" + valueSortBy, "");
    pathname = pathname.replace("&OrderBy=" + valueOrderBy, "").replace("?OrderBy=" + valueOrderBy, "");
    pathname = pathname.replace("&CountSort=" + valueCountSort, "").replace("?CountSort=" + valueCountSort, "");
    pathname = pathname.replace("&page=" + valuePage, "").replace("?page=" + valuePage, "");
    pathname = pathname.replace("?", "");
    var pathresult = pathname == null || pathname === "" ? "" : pathname + "&";

    if (pathresult[0] === "&") {
        pathresult = pathresult.substring(1, parseInt(pathresult.length));
    }

    if (valueSortBy == null) {
        window.location.href = "/" + nameArea + "/" + nameController + "/" + (actionName == null ? "Index" : actionName) + "?" + pathresult + "CountSort=" + valueSort;
    } else {
        window.location.href = "/" + nameArea + "/" + nameController + "/" + (actionName == null ? "Index" : actionName) + "?" + pathresult + "CountSort=" + valueSort + "&SortBy=" + valueSortBy + "&OrderBy=" + valueOrderBy;
    }
    
}

function RedirectOrderBy(sortBy, orderBy, nameController, nameArea) {
    var path = location.pathname.split('/');
    var actionName = path[4] != null ? path[4] : path[3];
    var valueSortBy = getUrlParameter("SortBy");
    var valueOrderBy = getUrlParameter("OrderBy");
    var valueCountSort = getUrlParameter("CountSort");
    var valuePage = getUrlParameter("page");
    var pathname = window.location.search.replace("&SortBy=" + valueSortBy, "").replace("?SortBy=" + valueSortBy, "");
    pathname = pathname.replace("&OrderBy=" + valueOrderBy, "").replace("?OrderBy=" + valueOrderBy, "");
    pathname = pathname.replace("&CountSort=" + valueCountSort, "").replace("?CountSort=" + valueCountSort, "");
    pathname = pathname.replace("&page=" + valuePage, "").replace("?page=" + valuePage, "");
    pathname = pathname.replace("?", "");
    var pathresult = pathname == null || pathname === "" ? "" : pathname + "&";


    if (pathresult[0] === "&") {
        pathresult = pathresult.substring(1, parseInt(pathresult.length));
    }


    if (valueCountSort == null) {
        window.location.href = "/" + nameArea + "/" + nameController + "/" + (actionName == null ? "Index" : actionName) + "?" + pathresult + "SortBy=" + sortBy + "&OrderBy=" + orderBy;
    } else {
        window.location.href = "/" + nameArea + "/" + nameController + "/" + (actionName == null ? "Index" : actionName) + "?" + pathresult + "CountSort=" + valueCountSort + "&SortBy=" + sortBy + "&OrderBy=" + orderBy;
    }
    
}

// ReSharper disable once InconsistentNaming
function ButtonImageClick(idNameInput)
{
    CKFinder.setupCKEditor(null, '/Plugins/ckfinder/');
    var resultInput = "#" + idNameInput;
    var finder = new CKFinder();
    finder.selectActionFunction = function (fileUrl) {
        $(resultInput).val(fileUrl);
    };
    finder.popup();
}

function ButtonImageAddToAlbumClick(idNameInput, idShowImage, strSelectorAlbumImage, strSelectorDescriptionImage) {
    CKFinder.setupCKEditor(null, '/Plugins/ckfinder/');
    var resultInput = "#" + idNameInput;
    var finder = new CKFinder();
    finder.selectActionFunction = function (fileUrl) {
        if ($(resultInput).val() != "")
        {
            let strValue = $(resultInput).val();
            $(resultInput).val(strValue + "," + fileUrl);
        }
        else
        {
            $(resultInput).val(fileUrl);
        }
        $(idShowImage).append("<div class='col-md-4'><div class='item-img' onclick='OpenPopupImage(this)'><img class='img-thumbnail' src='" + fileUrl + "' alt='" + fileUrl + "' /><input type='hidden' name='" + strSelectorAlbumImage + "' value='" + fileUrl + "' /><input type='hidden' name='" + strSelectorDescriptionImage + "' /></div><span class='icon-remove' onclick='RemoveImage(this)'><i class='fa fa-times'></i></span></div>");
    };
    finder.popup();
}

function UpdateThumbnailImage(srtSelector, strSelectorAlbumImage, strSelectorDescriptionImage, idShowImage)
{
    $(idShowImage).append("<div class='col-md-4'><div class='item-img' onclick='OpenPopupImage(this)'><img class='img-thumbnail' src='" + $(srtSelector).val() + "' alt='" + $(srtSelector).val() + "' /><input type='hidden' name='" + strSelectorAlbumImage + "' value='" + $(srtSelector).val() + "' /><input type='hidden' name='" + strSelectorDescriptionImage + "' /></div><span class='icon-remove' onclick='RemoveImage(this)'><i class='fa fa-times'></i></span></div>");
}

function SetupEditor(idNameInput)
{
    CKEDITOR.replace(idNameInput,
    {
        filebrowserBrowseUrl: '/Plugins/ckfinder/ckfinder.html',
        filebrowserImageBrowseUrl: '/Plugins/ckfinder/ckfinder.html?type=Images',
        filebrowserFlashBrowseUrl: '/Plugins/ckfinder/ckfinder.html?type=Flash',
        filebrowserUploadUrl: '/Plugins/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Files',
        filebrowserImageUploadUrl: '/Plugins/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Images',
        filebrowserFlashUploadUrl: '/Plugins/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Flash'
    });
}

function DirectUrl(strUrl)
{
    location.href = strUrl;
}