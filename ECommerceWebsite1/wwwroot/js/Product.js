var dtable;
$(document).ready(function () {
    dtable = $('#myTable').DataTable({
        "ajax": {
            "url": "/Admin/Product/AllProducts",
            "dataSrc": "data"
        },
        "columns": [
            { "data": 'name' },
            { "data": 'description' },
            { "data": 'price' },
            { "data": 'category.name' },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <a href="/Admin/Product/CreateUpdate?id=${data}" class="btn btn-sm btn-primary">
                            <i class="bi bi-pencil-square"></i> Edit
                        </a>
                        <a onclick=RemoveProduct("/Admin/Product/Delete/${data}") class="btn btn-sm btn-danger">
                            <i class="bi bi-trash"></i> Delete
                        </a>
                    `;
                }
            }
        ]
    });
});
function RemoveProduct(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You want to delete this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dtable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function (err) {
                    toastr.error("Something went wrong.");
                }
            });
        }
    });
}

