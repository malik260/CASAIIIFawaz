(function () {
	function formValidateRequired(id) {
		var el = document.getElementById(id);
		if (!el) return false;
		if (el.value.trim() === "") {
			el.style.border = '2px solid red';
			return false;
		}
		el.style.border = '';
		return true;
	}

	async function submitForm(form) {
		var url = form.getAttribute('action') || window.location.pathname;
		var btn = form.querySelector('button[type="submit"]');
		var formData = new FormData(form);

		// Use processingBtn from common.js if available
		if (typeof processingBtn === 'function' && btn && btn.id) {
			processingBtn(btn.id);
		}

		try {
			var resp = await fetch(url, {
				method: 'POST',
				headers: { 'X-Requested-With': 'XMLHttpRequest' },
				body: formData
			});
			var json = await resp.json();
			if (json && json.success) {
				if (typeof successAlert === 'function') {
					successAlert(json.message || 'Submitted');
				} else if (window.Swal) {
					Swal.fire('Success', json.message || 'Submitted', 'success');
				}
				form.reset();
			} else {
				var msg = (json && json.message) ? json.message : 'Submission failed';
				if (typeof errorAlert === 'function') errorAlert(msg); else if (window.Swal) Swal.fire('Error', msg, 'error');
			}
		} catch (e) {
			console.error(e);
			if (typeof errorAlert === 'function') errorAlert('An error occurred'); else if (window.Swal) Swal.fire('Error', 'An error occurred', 'error');
		} finally {
			if (typeof returnDefaultBtn === 'function' && btn && btn.id) {
				returnDefaultBtn(btn.id);
			}
		}
	}

	document.addEventListener('DOMContentLoaded', function () {
		console.log("=== DOMContentLoaded event fired ===");
		// Main contact form
		var mainForm = document.getElementById('mainContactForm');
		if (mainForm) {
			mainForm.addEventListener('submit', function (e) {
				e.preventDefault();

				var ok = true;
				ok = formValidateRequired('FirstName') && ok;
				ok = formValidateRequired('LastName') && ok;
				ok = formValidateRequired('Email') && ok;
				ok = formValidateRequired('Message') && ok;

				// validateEmail from common.js
				if (ok && typeof validateEmail === 'function') {
					ok = validateEmail('Email');
				}

				if (!ok) {
					if (typeof infoAlert === 'function') infoAlert('Please correct the highlighted fields');
					else if (window.Swal) Swal.fire('Validation', 'Please correct the highlighted fields', 'warning');
					return;
				}

				submitForm(mainForm);
			});
		}

		// Modal contact form
		var modalForm = document.getElementById('modalContactForm');
		if (modalForm) {
			modalForm.addEventListener('submit', function (e) {
				e.preventDefault();
				var ok = true;
				// modal_Name, modal_Email, modal_Message
				ok = formValidateRequired('modal_Name') && ok;
				ok = formValidateRequired('modal_Email') && ok;
				ok = formValidateRequired('modal_Message') && ok;

				if (ok && typeof validateEmail === 'function') {
					ok = validateEmail('modal_Email');
				}

				if (!ok) {
					if (typeof infoAlert === 'function') infoAlert('Please correct the highlighted fields');
					else if (window.Swal) Swal.fire('Validation', 'Please correct the highlighted fields', 'warning');
					return;
				}

				submitForm(modalForm);
			});
		}

		// Newsletter subscription form
		var newsletterForm = document.getElementById('newsletterForm');
		if (newsletterForm) {
			newsletterForm.addEventListener('submit', function (e) {
				e.preventDefault();

				var ok = formValidateRequired('newsletterEmail');

				// Validate email format
				if (ok && typeof validateEmail === 'function') {
					ok = validateEmail('newsletterEmail');
				}

				if (!ok) {
					if (typeof infoAlert === 'function') infoAlert('Please enter a valid email address');
					else if (window.Swal) Swal.fire('Validation', 'Please enter a valid email address', 'warning');
					return;
				}

				submitForm(newsletterForm);
			});
		}

		// Affiliate registration form
		var affiliateModal = document.getElementById('affiliateModal');
		if (!affiliateModal) {
			console.log('Affiliate modal not found');
		} else {
			console.log('Affiliate modal found, attaching listeners');

			var affiliateRegisterBtn = document.getElementById('affiliateRegisterBtn');
			var closeAffiliateModal = document.getElementById('closeAffiliateModal');
			var affiliateRegistrationForm = document.getElementById('affiliateRegistrationForm');

			if (affiliateRegisterBtn) {
				console.log('Register button found');
				affiliateRegisterBtn.addEventListener('click', function(e) {
					console.log('Register button clicked');
					e.preventDefault();
					affiliateModal.classList.add('opacity-100');
					affiliateModal.classList.remove('opacity-0', 'pointer-events-none');
				});
			} else {
				console.log('Register button NOT found');
			}

			if (closeAffiliateModal) {
				closeAffiliateModal.addEventListener('click', function(e) {
					e.preventDefault();
					affiliateModal.classList.remove('opacity-100');
					affiliateModal.classList.add('opacity-0', 'pointer-events-none');
				});
			}

			affiliateModal.addEventListener('click', function(e) {
				if (e.target === affiliateModal) {
					affiliateModal.classList.remove('opacity-100');
					affiliateModal.classList.add('opacity-0', 'pointer-events-none');
				}
			});

			if (affiliateRegistrationForm) {
				affiliateRegistrationForm.addEventListener('submit', function(e) {
					e.preventDefault();
					var ok = true;
					ok = formValidateRequired('aff_firstName') && ok;
					ok = formValidateRequired('aff_lastName') && ok;
					ok = formValidateRequired('email') && ok;
					ok = formValidateRequired('phone') && ok;
					ok = formValidateRequired('streetAddress') && ok;
					ok = formValidateRequired('accountName') && ok;
					ok = formValidateRequired('bankName') && ok;
					ok = formValidateRequired('accountNumber') && ok;

					// Validate email format
					if (ok && typeof validateEmail === 'function') {
						ok = validateEmail('email');
					}

					if (!ok) {
						if (typeof infoAlert === 'function') infoAlert('Please fill in all required fields.');
						else if (window.Swal) Swal.fire('Validation', 'Please fill in all required fields.', 'warning');
						return;
					}

					// Submit via AJAX with FormData
					var registerUrl = document.getElementById('registerAffiliateUrl').value;
					var formData = new FormData(affiliateRegistrationForm);

					fetch(registerUrl, {
						method: 'POST',
						headers: {
							'X-Requested-With': 'XMLHttpRequest'
						},
						body: formData
					})
					.then(response => response.json())
					.then(result => {
						if (result.success) {
							if (typeof successAlert === 'function') {
								successAlert(result.message);
							} else if (window.Swal) {
								Swal.fire('Success', result.message, 'success');
							}
							affiliateRegistrationForm.reset();
							affiliateModal.classList.remove('opacity-100');
							affiliateModal.classList.add('opacity-0', 'pointer-events-none');
						} else {
							if (typeof errorAlert === 'function') {
								errorAlert(result.message);
							} else if (window.Swal) {
								Swal.fire('Error', result.message, 'error');
							}
						}
					})
					.catch(error => {
						console.error('Error:', error);
						if (typeof errorAlert === 'function') {
							errorAlert('An error occurred. Please try again.');
						} else if (window.Swal) {
							Swal.fire('Error', 'An error occurred. Please try again.', 'error');
						}
					});
				});
			}
		}

		// Vendor Registration Form - BecomeAVendor page
		initVendorRegistrationForm();
	});

	// Initialize Vendor Registration Form
	function initVendorRegistrationForm() {
		console.log("initVendorRegistrationForm called");

		// Banner slider functionality
		const slides = document.querySelectorAll(".banner-slide");
		const dots = document.querySelectorAll(".banner-dot");

		if (slides.length > 0) {
			console.log("Banner slider found, initializing...");
			let current = 0;

			function showSlide(index) {
				slides.forEach((slide, i) => {
					slide.classList.toggle("opacity-100", i === index);
					slide.classList.toggle("opacity-0", i !== index);
					slide.classList.toggle("z-10", i === index);
					slide.classList.toggle("z-0", i !== index);
				});

				dots.forEach((dot, i) => {
					dot.classList.toggle("bg-white", i === index);
				});

				current = index;
			}

			dots.forEach(dot => {
				dot.addEventListener("click", () => {
					showSlide(parseInt(dot.dataset.index));
				});
			});

			setInterval(() => {
				showSlide((current + 1) % slides.length);
			}, 6000);
		}

		// Modal functionality - ALWAYS SET UP EVEN IF FORM IS NOT FOUND
		const openVendorModal = document.getElementById("openVendorModal");
		const closeVendorModal = document.getElementById("closeVendorModal");
		const vendorModal = document.getElementById("vendorModal");

		console.log("Modal elements found:", {
			openVendorModal: !!openVendorModal,
			closeVendorModal: !!closeVendorModal,
			vendorModal: !!vendorModal
		});

		if (openVendorModal && vendorModal) {
			console.log("Setting up openVendorModal click listener");
			openVendorModal.addEventListener("click", (e) => {
				console.log("openVendorModal clicked!");
				e.preventDefault();
				e.stopPropagation();
				console.log("Removing hidden class from vendorModal");
				vendorModal.classList.remove("hidden");
				console.log("Modal classes after click:", vendorModal.className);
			});
		} else {
			console.warn("Cannot set up modal - missing elements");
		}

		if (closeVendorModal && vendorModal) {
			closeVendorModal.addEventListener("click", (e) => {
				console.log("closeVendorModal clicked");
				e.preventDefault();
				e.stopPropagation();
				vendorModal.classList.add("hidden");
			});
		}

		if (vendorModal) {
			vendorModal.addEventListener("click", (e) => {
				if (e.target === vendorModal) {
					console.log("Clicked outside modal content, closing");
					vendorModal.classList.add("hidden");
				}
			});
		}

		// Form submission
		const vendorRegistrationForm = document.getElementById("vendorRegistrationForm");
		if (!vendorRegistrationForm) {
			console.log("vendorRegistrationForm not found - skipping form initialization");
			return;
		}

		const submitBtn = document.getElementById("submitBtn");
		let uploadedFile = null;

		// Validation rules
		const validators = {
			companyName: (value) => value.trim().length > 0 ? null : "Company name is required",
			contactPerson: (value) => value.trim().length > 0 ? null : "Contact person is required",
			email: (value) => {
				if (!value.trim()) return "Email is required";
				const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
				return emailRegex.test(value) ? null : "Valid email is required";
			},
			phoneNumber: (value) => value.trim().length > 0 ? null : "Phone number is required",
			tin: (value) => value.trim().length > 0 ? null : "TIN is required",
			businessCategory: (value) => value ? null : "Business category is required",
			businessAddress: (value) => value.trim().length > 0 ? null : "Business address is required"
		};

		// Validate individual field
		function validateField(name, value) {
			if (validators[name]) {
				return validators[name](value);
			}
			return null;
		}

		// Show field error
		function showFieldError(fieldName, errorText) {
			const input = vendorRegistrationForm.querySelector(`[name="${fieldName}"]`);
			if (input) {
				const container = input.closest("div");
				const errorSpan = container.querySelector(".error-text");

				if (errorText) {
					input.classList.add("border-red-500", "bg-red-50");
					if (errorSpan) {
						errorSpan.textContent = errorText;
						errorSpan.classList.remove("hidden");
					}
				} else {
					input.classList.remove("border-red-500", "bg-red-50");
					if (errorSpan) {
						errorSpan.classList.add("hidden");
					}
				}
			}
		}

		// Validate entire form
		function validateForm() {
			let isValid = true;
			const formData = new FormData(vendorRegistrationForm);

			// Validate all fields
			for (const [key, value] of formData.entries()) {
				if (key !== "document") {
					const error = validateField(key, value);
					if (error) {
						showFieldError(key, error);
						isValid = false;
					} else {
						showFieldError(key, null);
					}
				}
			}

			return isValid;
		}

		// Real-time validation on blur
		vendorRegistrationForm.querySelectorAll("input, textarea, select").forEach(field => {
			if (field.name && validators[field.name]) {
				field.addEventListener("blur", () => {
					const error = validateField(field.name, field.value);
					showFieldError(field.name, error);
				});
			}
		});

		// File upload handling (Single File)
		const uploadArea = document.getElementById("uploadArea");
		const documentInput = document.getElementById("documentInput");
		const uploadedFileDisplay = document.getElementById("uploadedFileDisplay");

		function updateUploadedFileDisplay() {
			uploadedFileDisplay.innerHTML = "";
			if (uploadedFile) {
				const displayHTML = `
				<div class="flex items-center justify-between bg-blue-50 p-2 rounded mt-2">
					<span class="text-sm text-gray-700">📄 ${uploadedFile.name}</span>
					<button type="button" class="text-red-500 hover:text-red-700 text-sm" onclick="removeUploadedFile()">Remove</button>
				</div>
			`;
				uploadedFileDisplay.innerHTML = displayHTML;
			}
		}

		window.removeUploadedFile = function () {
			uploadedFile = null;
			documentInput.value = "";
			updateUploadedFileDisplay();
		};

		if (uploadArea) {
			uploadArea.addEventListener("click", () => {
				documentInput.click();
			});

			uploadArea.addEventListener("dragover", (e) => {
				e.preventDefault();
				uploadArea.classList.add("bg-[#fff6ea]", "border-[#b39359]");
			});

			uploadArea.addEventListener("dragleave", () => {
				uploadArea.classList.remove("bg-[#fff6ea]", "border-[#b39359]");
			});

			uploadArea.addEventListener("drop", (e) => {
				e.preventDefault();
				uploadArea.classList.remove("bg-[#fff6ea]", "border-[#b39359]");

				const files = e.dataTransfer.files;
				if (files.length > 0) {
					const file = files[0];
					// Check file size (10MB)
					if (file.size > 10 * 1024 * 1024) {
						errorAlert(`File ${file.name} is too large. Maximum size is 10MB.`);
						return;
					}
					uploadedFile = file;
					updateUploadedFileDisplay();
				}
			});
		}

		if (documentInput) {
			documentInput.addEventListener("change", (e) => {
				const files = e.target.files;
				if (files.length > 0) {
					const file = files[0];
					// Check file size (10MB)
					if (file.size > 10 * 1024 * 1024) {
						errorAlert(`File ${file.name} is too large. Maximum size is 10MB.`);
						documentInput.value = "";
						return;
					}
					uploadedFile = file;
					updateUploadedFileDisplay();
				}
			});
		}

		// Helper function to set button to loading state
		function setLoadingBtn(btnId) {
			const btn = document.getElementById(btnId);
			if (btn) {
				btn.disabled = true;
				btn.innerHTML = '<span class="inline-flex items-center gap-2"><svg class="animate-spin h-5 w-5" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg>Submitting...</span>';
			}
		}

		// Helper function to return button to default state
		function returnDefaultBtn(btnId) {
			const btn = document.getElementById(btnId);
			if (btn) {
				btn.disabled = false;
				btn.innerHTML = "Submit Registration";
			}
		}

		// Form submission
		vendorRegistrationForm.addEventListener("submit", async (e) => { e.preventDefault();

			// Validate form
			if (!validateForm()) {
				errorAlert("Please fill in all required fields correctly.");
				return;
			}

			// Prepare form data
			const formData = new FormData();

			// Add all form fields
			formData.append("companyName", vendorRegistrationForm.querySelector('[name="companyName"]').value);
			formData.append("contactPerson", vendorRegistrationForm.querySelector('[name="contactPerson"]').value);
			formData.append("email", vendorRegistrationForm.querySelector('[name="email"]').value);
			formData.append("phoneNumber", vendorRegistrationForm.querySelector('[name="phoneNumber"]').value);
			formData.append("cacNumber", vendorRegistrationForm.querySelector('[name="cacNumber"]').value);
			formData.append("tin", vendorRegistrationForm.querySelector('[name="tin"]').value);
			formData.append("businessCategory", vendorRegistrationForm.querySelector('[name="businessCategory"]').value);
			formData.append("businessAddress", vendorRegistrationForm.querySelector('[name="businessAddress"]').value);

			// Add uploaded file if exists
			if (uploadedFile) {
				formData.append("document", uploadedFile);
			}

			// Disable submit button
			setLoadingBtn('submitBtn');

			try {
				// Send to backend
				const response = await fetch("/Home/SubmitVendorRegistration", { method: "POST", body: formData });

				const result = await response.json();

				if (!result.isError) {
					var url = window.location.pathname;
					successAlertWithRedirect(result.message, url);

					// Reset form and close modal
					vendorRegistrationForm.reset();
					uploadedFile = null;
					updateUploadedFileDisplay();

					// Close modal after success
					setTimeout(() => {
						vendorModal.classList.add("hidden");
					}, 1500);
				} else {
					returnDefaultBtn('submitBtn');
					errorAlert(result.message);
				}
			} catch (error) {
				console.error("Submission error:", error);
				returnDefaultBtn('submitBtn');
				errorAlert("Network error. Please check your connection and try again.");
			}
		});
	}
})();
