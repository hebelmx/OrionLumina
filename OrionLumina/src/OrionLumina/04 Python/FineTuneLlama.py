
import torch

# Print Torch version and CUDA version
print(f"Torch version: {torch.__version__}")
print(f"CUDA version: {torch.version.cuda}")

# Check if CUDA is available
if torch.cuda.is_available():
    print("CUDA is available.")
    print(f"Device Name: {torch.cuda.get_device_name(0)}")
    print(f"Device Count: {torch.cuda.device_count()}")
else:
    print("CUDA is not available.")
