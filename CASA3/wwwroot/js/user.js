function CreateUser() {
    // Validate required fields
    var firstName = $('#usr_firstname').val();
    var lastName = $('#usr_lastname').val();
    var email = $('#usr_email').val();
    var password = $('#usr_password').val();
    var role = $('#usr_role').val();

    if (!firstName || firstName.trim() === '') {
        errorAlert('First Name is required');
        return;
    }

    if (!lastName || lastName.trim() === '') {
        errorAlert('Last Name is required');
        return;
    }

    if (!email || email.trim() === '') {
        errorAlert('Email is required');
        return;
    }

    // Basic email validation
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
        errorAlert('Please enter a valid email address');
        return;
    }

    if (!password || password.trim() === '') {
        errorAlert('Password is required');
        return;
    }

    if (password.length < 3) {
        errorAlert('Password must be at least 3 characters long');
        return;
    }

    if (!role || role.trim() === '') {
        errorAlert('Role is required');
        return;
    }

    if (role !== 'Admin' && role !== 'Staff') {
        errorAlert('Role must be either Admin or Staff');
        return;
    }

    // Create FormData object
    var formData = new FormData();
    formData.append('FirstName', firstName.trim());
    formData.append('LastName', lastName.trim());
    formData.append('Email', email.trim());
    formData.append('Password', password);
    formData.append('Role', role);

    // Show processing state
    processingBtn('usr_add_user_btn');

    $.ajax({
        type: 'POST',
        url: '/User/CreateUser',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            returnDefaultBtn('usr_add_user_btn');
            if (result.success) {
                // Reset form
                $('#usr_addModal').modal('hide');
                $('#usr_firstname').val('');
                $('#usr_lastname').val('');
                $('#usr_email').val('');
                $('#usr_password').val('');
                $('#usr_role').val('');
                
                successAlertWithRedirect(result.message, window.location.pathname);
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            returnDefaultBtn('usr_add_user_btn');
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function GetUserDetailForEdit(id) {
    if (!id || id === '') {
        errorAlert('Invalid user ID');
        return;
    }

    $.ajax({
        type: 'GET',
        url: '/User/GetUserById',
        data: { id: id },
        success: function (result) {
            if (result.success) {
                var data = result.data;
                $('#usr_edit_Id').val(data.id);
                $('#usr_firstname_edit').val(data.firstName);
                $('#usr_lastname_edit').val(data.lastName);
                $('#usr_email_edit').val(data.email);
                $('#usr_role_edit').val(data.userRole);
                $('#usr_newpassword_edit').val(''); // Clear password field

                $('#usr_editModal').modal('show');
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function UpdateUser() {
    // Validate required fields
    var id = $('#usr_edit_Id').val();
    var firstName = $('#usr_firstname_edit').val();
    var lastName = $('#usr_lastname_edit').val();
    var email = $('#usr_email_edit').val();
    var role = $('#usr_role_edit').val();
    var newPassword = $('#usr_newpassword_edit').val();

    if (!id || id.trim() === '') {
        errorAlert('Invalid user ID');
        return;
    }

    if (!firstName || firstName.trim() === '') {
        errorAlert('First Name is required');
        return;
    }

    if (!lastName || lastName.trim() === '') {
        errorAlert('Last Name is required');
        return;
    }

    if (!email || email.trim() === '') {
        errorAlert('Email is required');
        return;
    }

    // Basic email validation
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
        errorAlert('Please enter a valid email address');
        return;
    }

    if (!role || role.trim() === '') {
        errorAlert('Role is required');
        return;
    }

    if (role !== 'Admin' && role !== 'Staff') {
        errorAlert('Role must be either Admin or Staff');
        return;
    }

    // Validate password if provided
    if (newPassword && newPassword.trim() !== '' && newPassword.length < 3) {
        errorAlert('Password must be at least 3 characters long');
        return;
    }

    // Create FormData object
    var formData = new FormData();
    formData.append('Id', id);
    formData.append('FirstName', firstName.trim());
    formData.append('LastName', lastName.trim());
    formData.append('Email', email.trim());
    formData.append('Role', role);
    
    // Only append password if provided
    if (newPassword && newPassword.trim() !== '') {
        formData.append('NewPassword', newPassword);
    }

    // Show processing state
    processingBtn('usr_edit_user_btn');

    $.ajax({
        type: 'POST',
        url: '/User/UpdateUser',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            returnDefaultBtn('usr_edit_user_btn');
            if (result.success) {
                successAlertWithRedirect(result.message, window.location.pathname);
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            returnDefaultBtn('usr_edit_user_btn');
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function getDataForDelete(id) {
    $('#usr_delete_Id').val(id);
    $('#usr_deleteModal').modal('show');
}

function DeleteUser() {
    var id = $('#usr_delete_Id').val();

    if (!id || id === '') {
        errorAlert('Invalid user ID');
        return;
    }

    processingBtn('usr_del_user_btn');

    $.ajax({
        type: 'POST',
        url: '/User/DeleteUser',
        data: { id: id },
        success: function (result) {
            returnDefaultBtn('usr_del_user_btn');
            if (result.success) {
                successAlertWithRedirect(result.message, window.location.pathname);
            } else {
                errorAlert(result.message);
            }
        },
        error: function (ex) {
            returnDefaultBtn('usr_del_user_btn');
            errorAlert('Network failure, please try again: ' + ex);
        }
    });
}

function ToggleUserStatus(id) {
    if (!id || id === '') {
        errorAlert('Invalid user ID');
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/User/ToggleUserStatus',
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
            'usr_add_user_btn': 'Add User',
            'usr_edit_user_btn': 'Update Record',
            'usr_del_user_btn': 'Delete'
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

