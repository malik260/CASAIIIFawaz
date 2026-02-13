function CreateCarousel() {
    // Validate required fields
    var title = $('#crl_title').val();
    var buttonText = $('#crl_button_text').val();
    var pageType = $('#crl_page_type').val();

    var backgroundImageInput = $('#crl_background_image')[0];
    var backgroundImage = backgroundImageInput && backgroundImageInput.files.length > 0 ? backgroundImageInput.files[0] : null;

    var brochureInput = $('#crl_brochure')[0];
    var brochure = brochureInput && brochureInput.files.length > 0 ? brochureInput.files[0] : null;

    if (!title || title.trim() === '') {
        errorAlert('Title is required');
        return;
    }

    if (!buttonText || buttonText.trim() === '') {
        errorAlert('Button Text is required');
        return;
    }

    if (!backgroundImage) {
        errorAlert('Background Image is required');
        return;
    }

    // Validate brochure for Home page type
    if (pageType == '1' && !brochure) { // 1 = Home
        errorAlert('Brochure (PDF) is required for Home carousel');
        return;
    }

    // Create FormData object
    var formData = new FormData();
    formData.append('Title', title);
    formData.append('Subtitle', $('#crl_subtitle').val() || '');
    formData.append('PageType', pageType);
    formData.append('ButtonText', buttonText);
    formData.append('BackgroundImage', backgroundImage);

    // Add badge text for Home
    if (pageType == '1') {
        formData.append('BadgeText', $('#crl_badge_text').val() || '');
        if (brochure) {
            formData.append('Brochure', brochure);
        }
    }

    // Add button link for Vendor
    if (pageType == '2') { // 2 = Vendor
        formData.append('ButtonLink', $('#crl_button_link').val() || '');
    }

    // Show processing state
    processingBtn('crl_add_carousel_btn');

    $.ajax({
        type: 'POST',
        url: '/Carousel/CreateCarousel',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            returnDefaultBtn('crl_add_carousel_btn');
            if (result.success) {
                successAlertWithRedirect(result.message, window.location.pathname);
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            returnDefaultBtn('crl_add_carousel_btn');
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function GetCarouselDetailForEdit(id) {
    if (!id || id === '') {
        errorAlert('Invalid carousel ID');
        return;
    }

    $.ajax({
        type: 'GET',
        url: '/Carousel/GetCarouselById',
        data: { id: id },
        success: function (result) {
            if (result.success) {
                var data = result.data;
                $('#crl_edit_Id').val(data.id);
                $('#crl_title_edit').val(data.title);
                $('#crl_subtitle_edit').val(data.subtitle);
                $('#crl_page_type_edit').val(data.pageType).trigger('change');
                $('#crl_button_text_edit').val(data.buttonText);
                $('#crl_badge_text_edit').val(data.badgeText);
                $('#crl_button_link_edit').val(data.buttonLink);
                $('#crl_display_order_edit').val(data.displayOrder);
                $('#crl_is_active_edit').prop('checked', data.isActive);

                // Display current background image
                if (data.backgroundImageUrl) {
                    $('#crl_current_background').html(`
                        <p class="text-muted mb-2">Current Background:</p>
                        <img src="/${data.backgroundImageUrl}" alt="Background" style="max-width: 300px; max-height: 150px; object-fit: cover; border: 1px solid #ddd; border-radius: 4px;">
                    `);
                }

                // Display current brochure link
                if (data.brochureUrl) {
                    $('#crl_current_brochure').html(`
                        <p class="text-muted">Current: <a href="/${data.brochureUrl}" target="_blank">View Brochure PDF</a></p>
                    `);
                }

                // Show/hide fields based on page type
                togglePageTypeFields('edit');

                $('#crl_editModal').modal('show');
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function UpdateCarousel() {
    // Validate required fields
    var id = $('#crl_edit_Id').val();
    var title = $('#crl_title_edit').val();
    var buttonText = $('#crl_button_text_edit').val();
    var pageType = $('#crl_page_type_edit').val();

    if (!id || id.trim() === '') {
        errorAlert('Invalid carousel ID');
        return;
    }

    if (!title || title.trim() === '') {
        errorAlert('Title is required');
        return;
    }

    if (!buttonText || buttonText.trim() === '') {
        errorAlert('Button Text is required');
        return;
    }

    // Create FormData object
    var formData = new FormData();
    formData.append('Id', id);
    formData.append('Title', title);
    formData.append('Subtitle', $('#crl_subtitle_edit').val() || '');
    formData.append('PageType', pageType);
    formData.append('ButtonText', buttonText);
    formData.append('DisplayOrder', $('#crl_display_order_edit').val() || '0');
    formData.append('IsActive', $('#crl_is_active_edit').is(':checked'));

    // Add badge text for Home
    if (pageType == '1') {
        formData.append('BadgeText', $('#crl_badge_text_edit').val() || '');

        // Add brochure if new one is selected
        var brochureInput = $('#crl_brochure_edit')[0];
        if (brochureInput && brochureInput.files.length > 0) {
            formData.append('Brochure', brochureInput.files[0]);
        }
    }

    // Add button link for Vendor
    if (pageType == '2') {
        formData.append('ButtonLink', $('#crl_button_link_edit').val() || '');
    }

    // Add background image if new one is selected
    var backgroundImageInput = $('#crl_background_image_edit')[0];
    if (backgroundImageInput && backgroundImageInput.files.length > 0) {
        formData.append('BackgroundImage', backgroundImageInput.files[0]);
    }

    // Show processing state
    processingBtn('crl_edit_carousel_btn');

    $.ajax({
        type: 'POST',
        url: '/Carousel/UpdateCarousel',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            returnDefaultBtn('crl_edit_carousel_btn');
            if (result.success) {
                successAlertWithRedirect(result.message, window.location.pathname);
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            returnDefaultBtn('crl_edit_carousel_btn');
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function getDataForDelete(id) {
    $('#crl_delete_Id').val(id);
    $('#crl_deleteModal').modal('show');
}

function DeleteCarousel() {
    var id = $('#crl_delete_Id').val();

    if (!id || id === '') {
        errorAlert('Invalid carousel ID');
        return;
    }

    processingBtn('crl_del_carousel_btn');

    $.ajax({
        type: 'POST',
        url: '/Carousel/DeleteCarousel',
        data: { id: id },
        success: function (result) {
            returnDefaultBtn('crl_del_carousel_btn');
            if (result.success) {
                successAlertWithRedirect(result.message, window.location.pathname);
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            returnDefaultBtn('crl_del_carousel_btn');
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function ToggleCarouselStatus(id) {
    if (!id || id === '') {
        errorAlert('Invalid carousel ID');
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/Carousel/ToggleCarouselStatus',
        data: { id: id },
        success: function (result) {
            if (result.success) {
                successAlertWithRedirect(result.message, window.location.pathname);
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

// Toggle fields based on page type selection
function togglePageTypeFields(mode) {
    var pageType = mode === 'add' ? $('#crl_page_type').val() : $('#crl_page_type_edit').val();
    var suffix = mode === 'add' ? '' : '_edit';

    if (pageType == '1') { // Home
        $('#crl_home_fields' + suffix).show();
        $('#crl_vendor_fields' + suffix).hide();
    } else if (pageType == '2') { // Vendor
        $('#crl_home_fields' + suffix).hide();
        $('#crl_vendor_fields' + suffix).show();
    }
}

// Initialize page type toggles on page load
$(document).ready(function () {
    $('#crl_page_type').on('change', function () {
        togglePageTypeFields('add');
    });

    $('#crl_page_type_edit').on('change', function () {
        togglePageTypeFields('edit');
    });

    // Trigger on initial load
    togglePageTypeFields('add');
});

// Helper functions (if not already defined globally)
function processingBtn(btnId) {
    var btn = $('#' + btnId);
    btn.prop('disabled', true);
    btn.data('original-text', btn.html());
    btn.html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...');
}

function returnDefaultBtn(btnId) {
    var btn = $('#' + btnId);
    var originalText = btn.data('original-text');

    if (!originalText) {
        var defaultTexts = {
            'crl_add_carousel_btn': 'Add Carousel',
            'crl_edit_carousel_btn': 'Update Record',
            'crl_del_carousel_btn': 'Delete'
        };
        originalText = defaultTexts[btnId] || 'Submit';
    }

    btn.prop('disabled', false);
    btn.html(originalText);
}

function successAlertWithRedirect(message, url) {
    if (typeof Swal !== 'undefined') {
        Swal.fire({
            icon: 'success',
            title: 'Success!',
            text: message,
            showConfirmButton: false,
            timer: 1500
        }).then(function () {
            window.location.href = url;
        });
    } else {
        alert(message);
        window.location.href = url;
    }
}

function errorAlert(message) {
    if (typeof Swal !== 'undefined') {
        Swal.fire({
            icon: 'error',
            title: 'Error!',
            text: message
        });
    } else {
        alert('Error: ' + message);
    }
}