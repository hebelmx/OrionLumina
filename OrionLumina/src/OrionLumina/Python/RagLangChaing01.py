from transformers import AutoModelForCausalLM, AutoTokenizer
model_name = "google-bert/bert-base-cased"
model = AutoModelForCausalLM.from_pretrained(model_name)
tokenizer = AutoTokenizer.from_pretrained(model_name)


from datasets import load_dataset

dataset = load_dataset(
  "DIBT/10k_prompts_ranked",
  split="train"
).filter(
  lambda r: r['avg_rating']>=4 and r['num_responses']>=2
)
dataset = dataset.to_list()
load_dataset = LoadDataFromDicts(
  name="load_dataset",
  data=dataset[0:500], # during development I used [0:1] to iterate quickly
  output_mappings={"prompt": "instruction"}
)
tokenized_dataset = dataset.map(lambda x: tokenizer(x['text']), batched=True)

from transformers import Trainer, TrainingArguments


training_args = TrainingArguments(
    output_dir="./results",
    evaluation_strategy="steps",
    per_device_train_batch_size=2,
    num_train_epochs=3,
    logging_dir='./logs',
)
trainer = Trainer(
    model=model,
    args=training_args,
    train_dataset=tokenized_dataset["train"],
    eval_dataset=tokenized_dataset["test"],
)
trainer.train()